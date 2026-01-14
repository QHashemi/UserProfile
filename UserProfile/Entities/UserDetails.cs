namespace UserProfile.Entities
{
    public class UserDetails
    {
        public Guid Id { get; set; }  // Primary key
        public Guid UserId { get; set; } // Foreign key to User
        public string? Address { get; set; } 
        public string? PhoneNumber { get; set; } 
        public string? MobileNumber { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
