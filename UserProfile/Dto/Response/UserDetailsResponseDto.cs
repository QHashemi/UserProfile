/// <summary>
/// Represents a user with their profile details.
/// </summary>
public class UserDetailsResponseDto
{
    /// <summary>User's unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>User's first name</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>User's last name</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>User's email address</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>User's role</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>User's address</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>User's phone number</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>User's mobile number</summary>
    public string MobileNumber { get; set; } = string.Empty;

    /// <summary>User's date of birth, null if not provided</summary>
    public DateTime? DateOfBirth { get; set; }
}
