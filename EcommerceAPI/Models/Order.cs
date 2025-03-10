namespace EcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }  = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        public User? User { get; set; }  
        public List<OrderProduct> OrderProducts { get; set; } = new();  // list of order products?
    }
}

