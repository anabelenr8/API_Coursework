using System.Text.Json.Serialization;

namespace EcommerceAPI.DTOs
{
    public class CartDTO
    { 
        public string UserId { get; set; } = string.Empty;
        
        [JsonIgnore] 
        public int Id { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }

    public class CartItemDTO  // 
    {
        public int ProductId { get; set; }  // Essential for mapping
        public int Quantity { get; set; }
    }
}