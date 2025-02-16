namespace EcommerceAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign key
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        // Navigation property
        public Order? Order { get; set; }
    }
}
