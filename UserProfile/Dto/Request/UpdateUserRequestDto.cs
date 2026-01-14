

namespace UserProfile.Dto.Request
{
    public class UpdateUserRequestDto
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string? Role { get; set; } 
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? MobileNumber { get; set; } 
        public DateTime? DateOfBirth { get; set; }

    }
}
