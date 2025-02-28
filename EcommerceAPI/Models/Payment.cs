namespace EcommerceAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign key reference to Order
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Navigation property
        public Order? Order { get; set; }
        public User? User { get; set; }
    }
}

