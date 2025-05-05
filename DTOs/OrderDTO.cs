
namespace EcommerceAPI.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        // Add these new properties to the DTO
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? PaymentMethod { get; set; }

        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
