using UserProfile.Dto.Request;
using UserProfile.Dto.Response;

namespace UserProfile.Services.UserServices
{
    public interface IUserService
    {
        Task<UserDetailsResponseDto?> UsersAsync();
        Task<UserDetailsResponseDto?> UserAsync(Guid id);
        Task<UpdateUserResponseDto?> UpdateUserAsync(Guid id, UpdateUserRequestDto request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}
