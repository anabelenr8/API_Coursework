namespace EcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Foreign key to User

        // FIX: Remove ProductId and use a list of CartItems
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Navigation property
        public User? User { get; set; }
    }
}
