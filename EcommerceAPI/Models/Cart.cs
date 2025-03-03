namespace EcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }  
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public User? User { get; set; }
    }
}
