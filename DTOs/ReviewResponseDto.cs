namespace HotByteAPI.DTOs
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }  // ✅ Must be here
        public int MenuItemId { get; set; }
        public string UserEmail { get; set; }
    }

}
