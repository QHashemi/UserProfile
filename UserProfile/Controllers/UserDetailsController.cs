
using Microsoft.AspNetCore.Mvc;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Services.UserServices;


namespace UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController(IUserService userService) : ControllerBase
    {

        // Get all users
        [HttpGet("users")]
        public async Task<ActionResult<List<UserDetailsResponseDto>>> GetAllUsers()
        {
            var users = userService.UsersAsync();
            return Ok(users);
        }

        // get User by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsResponseDto>> GetUserById(Guid id)
        {
            var user = userService.UserAsync(id);
            return Ok(user);
        }


        // Update user 
        [HttpPatch("{id}")]
        public async Task<ActionResult<UpdateUserResponseDto>> UpdateUser(Guid id, UpdateUserRequestDto request)
        {
            var updateUser = await userService.UpdateUserAsync(id, request);
            if (updateUser == null) 
            {
                throw new KeyNotFoundException("The User not Found!");
            }
            return updateUser;
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
