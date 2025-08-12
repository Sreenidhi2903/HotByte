namespace HotByteAPI.DTOs
{
    public class MenuItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int BranchId { get; set; }
    }
}
