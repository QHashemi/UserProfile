using Microsoft.AspNetCore.Mvc;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;

namespace UserProfile.Services.UserServices
{
    public interface IUserService
    {
        Task<List<UserDetailsResponseDto>?> UsersAsync();
        Task<UserDetailsResponseDto?> UserAsync(Guid id);
        Task<UserDetailsResponseDto?> UpdateUserAsync(Guid id, UpdateUserRequestDto request);
        Task<bool> DeleteUserAsync(Guid id);   
        Task<UpdateUserProfileResponseDto?> UpdateUserProfileAsync(UpdateUserProfileRequestDto request, Guid id);
    }
}
