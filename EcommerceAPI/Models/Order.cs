namespace EcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        public User? User { get; set; }  
        public List<OrderProduct> OrderProducts { get; set; } = new(); 
    }
}

