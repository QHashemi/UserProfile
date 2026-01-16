namespace UserProfile.Dto.Request
{
    public class PasswordResetRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

    }
}
