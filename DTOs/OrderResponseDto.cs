namespace HotByteAPI.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserEmail { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
