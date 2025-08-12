namespace HotByteAPI.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = "HotByte";
        public string Location { get; set; }
        public string ContactNumber { get; set; }
        public string RestaurantId { get; set; }  // Add this for ownership
        public User Restaurant { get; set; }

        public List<MenuItem> MenuItems { get; set; }
        public List<Order> Orders { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
