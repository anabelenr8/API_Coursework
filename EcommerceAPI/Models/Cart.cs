namespace EcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key
        public int ProductId { get; set; } // Foreign key
        public int Quantity { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Product? Product { get; set; }
    }
}
