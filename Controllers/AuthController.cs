using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Services;

namespace UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
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
            var serviceResult = await authService.LoginAsync(request);
            if (serviceResult is null)
            {
                return BadRequest("Invalid email or password.");
            }

            // return the response
            return Ok(serviceResult);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var serviceResponse = await authService.RefreshTokenAsync(request);
            if (serviceResponse is null || serviceResponse.AccessToken is null || serviceResponse.RefreshToken is null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            return Ok(serviceResponse);
        }



        // ENDPOINT WITH ONLY JWT =======================================================================>
        [Authorize]
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("The API is working!");
        }
        
        // ENDPOINT WITH JWT + ROLE =========================================================================>
        [Authorize(Roles="admin")]
        [HttpGet("test-role")]
        public ActionResult<string> TestRole()
        {
            return Ok("ROLE IS OK!");
        }

    }

}
