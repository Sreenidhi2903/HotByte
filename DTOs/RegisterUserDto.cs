namespace HotByteAPI.DTOs
{
    public class RegisterUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public string? City { get; set; }  // required only if Role == "Restaurant"

        public string RegistrationSecret { get; set; }  // For admin verification
    }
}

