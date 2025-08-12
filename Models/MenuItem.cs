
using System.Text.Json.Serialization;
namespace HotByteAPI.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int QuantityAvailable { get; set; }
        public bool IsCommon { get; set; } = false;


        public int? BranchId { get; set; }

        [JsonIgnore] // 👈 ignore during serialization/deserialization
        public Branch Branch { get; set; }

        [JsonIgnore]
        public List<OrderItem> OrderItems { get; set; }

        [JsonIgnore]
        public List<CartItem> CartItems { get; set; }

        [JsonIgnore]
        public List<Review> Reviews { get; set; }
        
    }
}
