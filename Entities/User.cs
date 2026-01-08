namespace UserProfile.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; }

    }
}
