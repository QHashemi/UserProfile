using UserProfile.Dto.Request;
using UserProfile.Entities;

namespace UserProfile.Services
{
    public interface IAuthService
    {
        // Register Interface
        Task<User?> RegisterAsync(RegisterRequestDto request);

        Task<User?> LoginAsync(LoginRequestDto request);
    }
}
