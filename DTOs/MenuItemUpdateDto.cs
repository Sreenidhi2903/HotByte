public class MenuItemUpdateDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int QuantityAvailable { get; set; }
    public string ImageUrl { get; set; }
    public int BranchId { get; set; }
}
