namespace UserProfile.Dto.Request
{
    public class RegisterRequestDto
    {
        public required string firstname { get; set; }
        public required string lastname { get; set; } 
        public required string email { get; set; } 
        public required string password { get; set; }  
        public required string confirmPassword { get; set; } 
    }
}
