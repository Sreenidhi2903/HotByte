namespace HotByteAPI.DTOs
{
    public class CustomerProfileUpdateDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public ShippingAddressDto? ShippingAddress { get; set; }
    }
}
