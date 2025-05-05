using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class OrderProduct
    {
        [Key]  // Primary Key
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at time of order

        // Navigation Properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}


