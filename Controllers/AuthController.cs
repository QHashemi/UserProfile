using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;
using UserProfile.Services;

namespace UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
    {

        // Register 
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {

            // register user in service
            var user = await authService.RegisterAsync(request);

            // check if the user has been registered successfully
            if (user is null)
            {
                return BadRequest("User registration failed.");
            }
            // return the response

            return Ok(new RegisterResponseDto
            {
                User = user,
            });
        }

        // Login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request) 
        {

            // login user in service
            var user = await authService.LoginAsync(request);
            if (user is null)
            {
                return BadRequest("Invalid email or password.");
            }
            // generate Access Token
            var token = GenerateJwtAccessToken(user);

            // return the response
            return Ok(new LoginResponseDto {
                AccessToken = token,
                User = user
            });
        }


        // ENDPOINT WITH ONLY JWT =======================================================================>
        [Authorize]
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("The API is working!");
        }
        
        [Authorize(Roles="admin")]
        [HttpGet("test-role")]
        public ActionResult<string> TestRole()
        {
            return Ok("ROLE IS OK!");
        }



        // Generate JWT Access Token
        private string GenerateJwtAccessToken(User user)
        {

            // create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.firstname} {user.lastname}"),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
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

        private string GetAppSettings(string setting)
        {
            var appSettingsString = $"AppSettings:{setting}";
            return configuration.GetValue<string>(appSettingsString)!;
        }


    }



}
