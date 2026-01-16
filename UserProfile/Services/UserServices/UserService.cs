
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;
using UserProfile.Services.LoggerService;

namespace UserProfile.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ICustomLoggerService _logger;

         public UserService(AppDbContext context, ICustomLoggerService logger)
        {
            this._context = context;
            _logger = logger;
        }


        // Get all Users
        public async Task<List<UserDetailsResponseDto>?> UsersAsync()
        {
            var users = await _context.Users.Include(detail => detail.UserDetails).ToListAsync();

            var response = users.Select(u => ReturnUserResponse(u)).ToList();
            return response;
        }


        // get single user
        public async Task<UserDetailsResponseDto?> UserAsync(Guid id)
        {
            var user = await _context.Users.Include(user => user.UserDetails).FirstOrDefaultAsync(user => user.Id == id);

            if (user is null)
            {
                await _logger.Warning(message: $"The User with Id :{id} is not found", logEvent: "GET-SINGLE-USER", statusCode: 404);
                throw new KeyNotFoundException("The user is not found");
            }
            var response = ReturnUserResponse(user);
            return response;
        }


        // Update user by id
        public async Task<UserDetailsResponseDto?> UpdateUserAsync(Guid id,UpdateUserRequestDto request)
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
                userToEdit.UserDetails = new UserDetail
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
            return ReturnUserResponse(userToEdit);
        }



        public async Task<UpdateUserProfileResponseDto?> UpdateUserProfileAsync(UpdateUserProfileRequestDto request, Guid id)
        {
            // Fetch user including UserDetails
            var user = await _context.Users.Include(u => u.UserDetails).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                await _logger.Warning( message: $"User with Id {id} not found",logEvent: "UPDATE-PROFILE",statusCode: 404);
                // Use ArgumentException or custom NotFoundException
                throw new KeyNotFoundException("The user is not found");
            }

            // Validate file
            if (request.UserProfileImg == null || request.UserProfileImg.Length == 0)
            {
                throw new ArgumentException("Profile image file is required", nameof(request.UserProfileImg));
            }

            // Ensure Uploads/Profiles directory exists
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Profiles");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Generate a safe, unique file name
            var fileName = $"{user.FirstName.ToLower()}-{user.LastName.ToLower()}{Path.GetExtension(request.UserProfileImg.FileName)}";
            var filePath = Path.Combine(uploadDir, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.UserProfileImg.CopyToAsync(stream);
            }

            // Ensure UserDetails exists
            if (user.UserDetails == null)
            {
                user.UserDetails = new UserDetail
                {
                    UserId = user.Id
                };
            }

            // Update ProfileImage with relative path (frontend-friendly)
            user.UserDetails.ProfileImage = Path.Combine("Uploads", "Profiles", fileName).Replace("\\", "/");

            // Save changes
            await _context.SaveChangesAsync();

            // Return DTO with relative path
            return new UpdateUserProfileResponseDto
            {
                UserProfileImg = user.UserDetails.ProfileImage
            };
        }


        // delete User
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

        private static UserDetailsResponseDto ReturnUserResponse(User user)
        {
            return new UserDetailsResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Address = user.UserDetails?.Address ?? "",
                PhoneNumber = user.UserDetails?.PhoneNumber ?? "",
                MobileNumber = user.UserDetails?.MobileNumber ?? "",
                DateOfBirth = user.UserDetails?.DateOfBirth
            };
        }
        
    }
}
