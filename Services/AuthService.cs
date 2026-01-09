using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;


namespace UserProfile.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<User> RegisterAsync(RegisterRequestDto request)
        {
            // Check if the user already exists
            var existingUser = await context.Users.AnyAsync(user => user.Email == request.Email);
            if (existingUser)
            {
                return null;
            }

            // Check if the user password and confirm password match
            if (request.Password != request.ConfirmPassword)
            {
                return null;
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

            // return user
            return user;
        }



        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Check if the user exists
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                return null;
            }

            // verify password
            var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if(passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }


            // Generate Tokens
            var accessToken = GenerateJwtAccessToken(user);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

            // return user
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user,
            };
        }


        // Generate Refresh and AccessToken
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            // validate Refresh Token
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            // generate and save new refresh token
            var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user);
            var newAccessToken = GenerateJwtAccessToken(user);
            return new LoginResponseDto
            {
                User = user,
                RefreshToken = newRefreshToken,
                AccessToken = newAccessToken
            };

        }


        // ACCESS TOKEN HANDLER =====================================================================>

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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetAppSettings("Token")));

            // create credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);


            // create token
            var tokenDescriptor = new JwtSecurityToken(
               issuer: GetAppSettings("Issuer"),
               audience: GetAppSettings("Audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1),
               signingCredentials: credentials
            );

            // return token
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }




        
   
        // Refresh Token Handler =================================================================>
        // Generate Refresh Token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var range = RandomNumberGenerator.Create();
            range.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);
            return refreshToken;
        }
        // Generate and Save Refresh Token
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
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
                return null;
            }
            return user;
        }



        // GENERAL FUNCTION ========================================================================>
        private string GetAppSettings(string setting)
        {
            var appSettingsString = $"AppSettings:{setting}";
            return configuration.GetValue<string>(appSettingsString)!;
        }


    }
}
