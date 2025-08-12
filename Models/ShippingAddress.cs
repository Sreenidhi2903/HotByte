namespace HotByteAPI.Models
{
    public class ShippingAddress
    {
        public int Id { get; set; }

        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        
    }
}
