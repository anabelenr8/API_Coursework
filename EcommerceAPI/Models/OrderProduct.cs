using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class OrderProduct
    {
        [Key] // Ensure Id exists
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; } 

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}

