using UserProfile.Entities;

namespace UserProfile.Dto.Response
{
    public class LoginResponseDto
    {
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
