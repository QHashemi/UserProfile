
using Microsoft.EntityFrameworkCore;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;
using UserProfile.Utils.Interfaces;

namespace UserProfile.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ICustomLogger _logger;

         public UserService(AppDbContext context, ICustomLogger logger)
        {
            this._context = context;
            _logger = logger;
        }


        // Get all Users
        public async Task<List<UserDetailsResponseDto>> UsersAsync()
        {
            var users = await _context.Users.Include(detail => detail.UserDetails).ToListAsync();

            var response = users.Select(u => new UserDetailsResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                Address = u.UserDetails?.Address ?? "",
                PhoneNumber = u.UserDetails?.PhoneNumber ?? "",
                MobileNumber = u.UserDetails?.MobileNumber ?? "",
                DateOfBirth = u.UserDetails?.DateOfBirth ?? DateTime.MinValue
            }).ToList();

            return response;
        }

        public async Task<UserDetailsResponseDto?> UserAsync(Guid id)
        {
            var user = await _context.Users.Include(user => user.UserDetails).FirstOrDefaultAsync(user => user.Id == id);

            if (user is null)
            {
                await _logger.Warning(message: $"The User with Id :{id} is not found", logEvent: "GET-SINGLE-USER", statusCode: 404);
                throw new KeyNotFoundException("The user is not found");
            }
            var response = new UserDetailsResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Address = user.UserDetails?.Address ?? "",
                PhoneNumber = user.UserDetails?.PhoneNumber ?? "",
                MobileNumber = user.UserDetails?.MobileNumber ?? "",
                DateOfBirth = user.UserDetails?.DateOfBirth ?? DateTime.MinValue,
            };
            return response;
        }


        // Update user by id
        public async Task<UpdateUserResponseDto?> UpdateUserAsync(Guid id,UpdateUserRequestDto request)
        {
            var userToEdit = await _context.Users
                .Include(u => u.UserDetails)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userToEdit == null)
            {
                await _logger.Warning(message: "The User not found in database", statusCode: 404, logEvent: "UPDATE-USER");
                throw new KeyNotFoundException("The user is not found");
            }

            // Update User
            userToEdit.FirstName = request.FirstName ?? userToEdit.FirstName;
            userToEdit.LastName = request.LastName ?? userToEdit.LastName;
            userToEdit.Role = request.Role ?? userToEdit.Role;

            // Ensure UserDetails exists
            if (userToEdit.UserDetails == null)
            {
                userToEdit.UserDetails = new UserDetails
                {
                    UserId = userToEdit.Id
                };
            }

            // Update UserDetails
            userToEdit.UserDetails.Address =
                request.Address ?? userToEdit.UserDetails.Address;

            userToEdit.UserDetails.PhoneNumber =
                request.PhoneNumber ?? userToEdit.UserDetails.PhoneNumber;

            userToEdit.UserDetails.MobileNumber =
                request.MobileNumber ?? userToEdit.UserDetails.MobileNumber;

            userToEdit.UserDetails.DateOfBirth =
                request.DateOfBirth ?? userToEdit.UserDetails.DateOfBirth;

            await _context.SaveChangesAsync();

            // Return response
            return new UpdateUserResponseDto
            {
                Id = userToEdit.Id,
                FirstName = userToEdit.FirstName,
                LastName = userToEdit.LastName,
                Email = userToEdit.Email,
                Role = userToEdit.Role,
                Address = userToEdit.UserDetails.Address,
                PhoneNumber = userToEdit.UserDetails.PhoneNumber,
                MobileNumber = userToEdit.UserDetails.MobileNumber,
                DateOfBirth = userToEdit.UserDetails.DateOfBirth
            };
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var userToDelete = await _context.Users
             .Include(user => user.UserDetails)
             .FirstOrDefaultAsync(user => user.Id == id);
            if (userToDelete == null)
            {
                await _logger.Warning(message: "The User not found in database", statusCode: 404, logEvent: "UPDATE-USER");
                throw new  KeyNotFoundException("The user is not found");
            }
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return true;
        }

        
    }
}
