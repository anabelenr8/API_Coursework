namespace EcommerceAPI.DTOs
{
    public class PaymentDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public required decimal Amount { get; set; }
        public required string PaymentMethod { get; set; } = string.Empty;
        public required string Status { get; set; } = "Pending";
    }
}
