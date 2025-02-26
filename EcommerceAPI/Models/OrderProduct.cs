namespace EcommerceAPI.Models
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // ✅ Navigation Properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
