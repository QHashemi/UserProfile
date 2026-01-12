using System.ComponentModel.DataAnnotations;

namespace UserProfile.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; }
        public string Role { get; set; } = string.Empty;
        public string RefreshToken {  get; set; }  = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
