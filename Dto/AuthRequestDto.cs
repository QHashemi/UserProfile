namespace UserProfile.Dto
{
    public class AuthRequestDto
    {
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; }
        public string confirmPassword { get; set; }
    }
}
