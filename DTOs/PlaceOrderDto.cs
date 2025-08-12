using HotByteAPI.Models;

namespace HotByteAPI.DTOs
{
    public class PlaceOrderDto
    {
        public int BranchId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
    }
}
