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
            if (user == null)
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


    }



}
