
namespace EcommerceAPI.DTOs
{
    public class OrderDTO
    {
    
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO 
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
