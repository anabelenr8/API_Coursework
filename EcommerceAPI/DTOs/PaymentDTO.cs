namespace EcommerceAPI.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public required decimal Amount { get; set; }  // ✅ Required modifier added
        public required string PaymentMethod { get; set; } = string.Empty;  // ✅ Required added
        public required string Status { get; set; } = string.Empty;  // ✅ Required added
    }
}


