using System;

namespace EcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        // Navigation property: Each order belongs to a user
        public User? User { get; set; }
    }
}
