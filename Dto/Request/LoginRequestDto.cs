

namespace UserProfile.Dto.Request
{
    public class LoginRequestDto
    {
        public required string email { get; set; }
        public required string password { get; set; }
        public required string role { get; set; }

    }
}
