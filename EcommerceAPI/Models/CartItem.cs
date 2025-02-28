namespace EcommerceAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }  // Foreign Key
        public int ProductId { get; set; }  // Foreign Key
        public int Quantity { get; set; }

        // Navigation properties
        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
