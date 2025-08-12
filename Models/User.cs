using HotByteAPI.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; } = "User";

    // Optional (you can keep this directly or via ShippingAddress)
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }  // nullable for Users/Admins who don't need this


    public List<Order> Orders { get; set; }
    public List<Review> Reviews { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
}
