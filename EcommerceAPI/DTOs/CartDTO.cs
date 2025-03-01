namespace EcommerceAPI.DTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartProductDTO> Items { get; set; } = new();
    }

    public class CartProductDTO  // 
    {
        public int ProductId { get; set; }  // Essential for mapping
        public int Quantity { get; set; }
    }
}
