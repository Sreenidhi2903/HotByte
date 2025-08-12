namespace HotByteAPI.DTOs
{
    public class ReviewDto
    {
        public int MenuItemId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; }
    }
}
