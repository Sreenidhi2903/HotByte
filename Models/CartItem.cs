using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotByteAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public decimal DiscountedPricePerItem => MenuItem == null
            ? 0
            : decimal.Round(MenuItem.Price * (1 - ((MenuItem.DiscountPercentage ?? 0) / 100)), 2);

        [NotMapped]
        public decimal TotalPrice => decimal.Round(DiscountedPricePerItem * Quantity, 2);
    }
}
