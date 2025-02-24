using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class OrderProduct
    {
        public int OrderId { get; set; } // FK to Order
        public int ProductId { get; set; } // FK to Product

        // Navigation Properties
        [ForeignKey("OrderId")]
        public required Order Order { get; set; }

        [ForeignKey("ProductId")]
        public required Product Product { get; set; }

        public int Quantity { get; set; } // Number of items in this order
    }
}
