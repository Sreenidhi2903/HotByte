using System;
using System.ComponentModel.DataAnnotations;

  namespace HotByteAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Preparing, Delivered
        public PaymentMethod PaymentMethod { get; set; } // COD or Prepaid
        public decimal TotalAmount { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}

