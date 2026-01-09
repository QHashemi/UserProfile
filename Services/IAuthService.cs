using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Entities;

namespace UserProfile.Services
{
    public interface IAuthService
    {
        // Register Interface
        Task<User?> RegisterAsync(RegisterRequestDto request);

        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
