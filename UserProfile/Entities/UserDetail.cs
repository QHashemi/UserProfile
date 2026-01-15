using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfile.Entities
{
    public class UserDetail
    {
        public Guid Id { get; set; }  // Primary key

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // One-to-one relationship
        public User User { get; set; } = null!;
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; } // Foreign key
    }
}
