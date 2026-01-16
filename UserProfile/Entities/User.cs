namespace UserProfile.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Optional refresh token
        public string? RefreshToken { get; set; } = null;
        public DateTime? RefreshTokenExpiryTime { get; set; } = null;

        public string? ResetPasswordToken { get; set; } 
        public DateTime? ResetPasswordTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        // Relationship =========================================>

        // one-to-one Relation with UserDetails
        public UserDetail? UserDetails { get; set; }

        // one-to-many (User has many Posts)
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
