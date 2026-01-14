
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;
using UserProfile.Services.UserServices;


namespace UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController(IUserService userService, AppDbContext context) : ControllerBase
    {

        // Get all users
        [HttpGet("users")]
        public async Task<ActionResult<List<UserDetailsResponseDto>>> GetAllUsers()
        {
            var users = await userService.UsersAsync();
            return Ok(users);
        }

        // get User by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsResponseDto>> GetUserById(Guid id)
        {
            var user = await userService.UserAsync(id);
            return Ok(user);
        }


        // Update user 
        [HttpPatch("{id}")]
        public async Task<ActionResult<UserDetailsResponseDto>> UpdateUser(Guid id, UpdateUserRequestDto request)
        {
            var updateUser = await userService.UpdateUserAsync(id, request);
            if (updateUser == null) 
            {
                throw new KeyNotFoundException("The User not Found!");
            }
            return updateUser;
        }


        // Update user Profile Profile image
        [HttpPost("profile/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UpdateUserProfileResponseDto>> UpdateUserProfile([FromForm] UpdateUserProfileRequestDto request, Guid id){

            var filePath = await userService.UpdateUserProfileAsync(request, id);

            return Ok(filePath);
        }


        // Delete User by Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            var userDeleted = await userService.DeleteUserAsync(id);
            if(!userDeleted)
            {
                throw new KeyNotFoundException("The User not Found");
            }
            return NoContent();
        }




    }
}
