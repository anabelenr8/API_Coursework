namespace EcommerceAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public decimal Amount { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        

        public Order? Order { get; set; }
        public User? User { get; set; }
    }
}

