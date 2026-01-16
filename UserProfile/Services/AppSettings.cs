namespace UserProfile.Services
{
    public class AppSettings
    {
        public string Token { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}
