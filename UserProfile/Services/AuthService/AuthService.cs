
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;
using UserProfile.Services.EmailService;
using UserProfile.Services.LoggerService;
using UserProfile.Utils;


namespace UserProfile.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext context;
        private readonly AppSettings appSettings;
        private readonly ICustomLoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
  


        public AuthService(
            AppDbContext context,
            ICustomLoggerService logger,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IOptions<AppSettings> appSettings
            )
        {
            this.context = context;
            this.appSettings = appSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
           
        }


        public async Task<User> RegisterAsync(RegisterRequestDto request)
        {
            // Check if the user already exists
            var existingUser = await context.Users.AnyAsync(user => user.Email == request.Email);
            if (existingUser)
            {

                await _logger.Warning(
                    message: "Registration failed: user already exists",
                    logEvent: "AUTH_REGISTER_FAILED"
                );
                throw new InvalidOperationException("User already exists.");
            }


            // Check if the user password and confirm password match
            if (request.Password != request.ConfirmPassword)
            {
                await _logger.Warning(
                   message: "Registration failed: password mismatch",
                   logEvent: "AUTH_REGISTER_FAILED"
               );
                throw new UnauthorizedAccessException("Password is not match");
            }

            // if not exits create new User
            var user = new User();

            // hash password
            var hashPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            // set the user properties
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PasswordHash = hashPassword;

            // check if the table is empty
            var isUserTableEmpty = await context.Users.AnyAsync();
            if (!isUserTableEmpty) // No users yet
            {
                user.Role = "admin";
            }
            else
            {
                user.Role = "user";
            }


            // set use to db
            await context.Users.AddAsync(user);
            // save db changes
            await context.SaveChangesAsync();

            await _logger.Info(
                message: "User registered successfully",
                logEvent: "AUTH_REGISTER_SUCCEEDED",
                userIdentifier: user.Id.ToString()
            );

            // return user
            return user;
        }



        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Get client IP address (used for rate limiting & security)
            var clientIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Create a unique key per IP + email to track login attempts
            var attemptKey = $"{clientIp}:{request.Email?.ToLowerInvariant() ?? "unknown"}";


            // Block login if too many failed attempts occurred
            if (LoginAttemptTracker.IsBlocked(attemptKey))
            {
                // Log blocked login attempt
                await _logger.Warning(message: "Login blocked due to too many failed attempts",logEvent: "AUTH_LOGIN_BLOCKED");
                throw new UnauthorizedAccessException("Too many login attempts. Try again later.");
            }


            // Try to find user by email
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // If user does not exist
            if (user is null)
            {
                // Record failed login attempt
                LoginAttemptTracker.RecordFailure(attemptKey);
                await _logger.Warning(message: "Login failed: invalid credentials",logEvent: "AUTH_LOGIN_FAILED");
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Verify submitted password against stored hash
            var verificationResult = new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            // If password is incorrect
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                // Record failed login attempt
                LoginAttemptTracker.RecordFailure(attemptKey);
                await _logger.Warning( message: "Login failed: invalid credentials",logEvent: "AUTH_LOGIN_FAILED");
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Login succeeded → reset failed attempt counter
            LoginAttemptTracker.Reset(attemptKey);


            var accessToken = GenerateJwtAccessToken(user);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user, 512);
            await _logger.Info(message: "Login succeeded",logEvent: "AUTH_LOGIN_SUCCEEDED", userIdentifier: user.Id.ToString());

            // Return authentication response
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };
        }




        // Generate Refresh and AccessToken
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            // validate Refresh Token
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                await _logger.Warning(message: "Refresh token validation failed",logEvent: "REFRESH_TOKEN_FAILED",userIdentifier: request.UserId.ToString());
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }
            // generate and save new refresh token
            var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user, 512);
            var newAccessToken = GenerateJwtAccessToken(user);


            await _logger.Info( message: "Refresh token succeeded",logEvent: "REFRESH_TOKEN_SUCCEEDED",userIdentifier: user.Id.ToString());
            return new LoginResponseDto
            {
                User = user,
                RefreshToken = newRefreshToken,
                AccessToken = newAccessToken
            };

        }
        // Reset password
        public async Task<PasswordResetResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);

            // Email not exits send back Not Found
            if (user is null)
            {
                await _logger.Warning(message: "Email for password reset is not found", statusCode: 404, logEvent: "RESET-PASSWORD-FAILED");
                throw new KeyNotFoundException($"The Email you have {user?.Email} is not working!");
            }

            // Genearte token with userDate
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hashedToken = HashToken(token);

            var expireTime = DateTime.UtcNow.AddMinutes(15);

            // save into db
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = expireTime;

            var resetLink = $"http://localhost:3000/reset-password?token={token}";


            // Send Email to the user with LINK
            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                $@"
                <h3>Rest Passowrd</h3>
                <p>Click the link below to reset your password:</p>
                <p>LINK: <a href='{resetLink}'>Reset Password</a></p>
                <p>This link will be expires in 15 Minutes.</p>"
            );

            // Set into DB
            await context.SaveChangesAsync();

            await _logger.Info(message: "Email has been send to the user for password reset", logEvent: "RESET-PASSWORD", userIdentifier: user.Id.ToString());
            return new PasswordResetResponseDto
            {
                Message = $"The Email has been send to {user.Email}",
                IsPasswordReset = true,
            }; ;
        }


        // Reset Password 
        public async Task<PasswordResetResponseDto> ResetPasswordAsync(PasswordResetRequestDto request)
        {
            // check if the link is not expired
            var user = await context.Users.FirstOrDefaultAsync(user => user.ResetPasswordToken == request.token && user.ResetPasswordTokenExpiry > DateTime.UtcNow);
            if (user is null)
            {
                await _logger.Warning(message: "The Link for Password Reset has been expired", statusCode: 408, logEvent: "RESET-PASSWORD-FAILED");
                throw new TimeoutException("Reset link is invalid or expired.");
            };

            // reset password
            // has password
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.NewPassword);

            user.PasswordHash = hashedPassword;

            await context.SaveChangesAsync();
            await _logger.Info(message: "The Password has been reseted successfully.", logEvent: "RESET-PASSWORD-SUCCESS", userIdentifier: user.Id.ToString());

            return new PasswordResetResponseDto
            {
                Message = "The Password has been reseted",
                IsPasswordReset = true,
            };
        }



        // ACCESS TOKEN HANDLER =====================================================================>
        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        // Generate JWT Access Token
        private string GenerateJwtAccessToken(User user)
        {

            // create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // create Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Token));

            // create credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // create token
            var tokenDescriptor = new JwtSecurityToken(
               issuer: appSettings.Issuer,
               audience: appSettings.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1),
               signingCredentials: credentials
            );

            // return token
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

   
        // Refresh Token Handler =================================================================>
        // Generate Refresh Token
        private string GenerateRefreshToken(int bit)
        {
            var randomBytes = new byte[bit];
            using var range = RandomNumberGenerator.Create();
            range.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);
            return refreshToken;
        }
        // Generate and Save Refresh Token
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user,int bit)
        {
            var refreshToken = GenerateRefreshToken(bit);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        private async Task<User?> ValidateRefreshTokenAsync(Guid UserId, string refreshToken)
        {
            var user = await context.Users.FindAsync(UserId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                await _logger.Warning(message: "Invalid or expired refresh token.", logEvent: "REFRESH-TOKEN-FAILED");
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            await _logger.Info(
                 message: "Refresh token succeeded",
                 logEvent: "REFRESH_TOKEN_SUCCEEDED",
                 userIdentifier: user.Id.ToString()
             );
            return user;
        }




    }
}
