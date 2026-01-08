using Microsoft.AspNetCore.Http;
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
                firstname = request.firstname,
                lastname = request.lastname,
                email = request.email
            });
        }

       // Login

        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request) 
        {

            // login user in service
            var user = await authService.LoginAsync(request);
            if (user is null)
            {
                return BadRequest("Invalid email or password.");
            }

            // return the response
            return Ok(new LoginResponseDto {
                firsname = user.firstname,
                lastname = user.lastname,
                email = request.email
            });
        }


    }



}
