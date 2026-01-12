using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Services.AuthService;

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

            // return the response
            return Ok(serviceResult);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var serviceResponse = await authService.RefreshTokenAsync(request);
          
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
