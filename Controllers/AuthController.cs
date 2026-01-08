using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                firstname = request.firstname,
                lastname = request.lastname,
                email = request.email
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
                firsname = user.firstname,
                lastname = user.lastname,
                email = request.email
            });
        }


       // Generate JWT Access Token
        private string GenerateJwtAccessToken(User user)
        {

            // create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.firstname} {user.lastname}"),
                new Claim(ClaimTypes.Email, user.email)
            };

            // create Key
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(GetAppSettings("token")));

            // create credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);


            // create token
            var token = new JwtSecurityToken(
                    issuer: GetAppSettings("issuer"),
                    audience: GetAppSettings("audience"),
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

            // return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetAppSettings(string setting)
        {
            var appSettingsString = $"AppSettings:{setting}";
            return configuration.GetValue<string>(appSettingsString)!;
        }


    }



}
