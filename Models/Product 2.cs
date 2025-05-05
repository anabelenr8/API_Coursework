using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EcommerceAPI.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] // Makes the Name property required
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Specifies the data type for Price in the database
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Description { get; set; } // Nullable property for description
        public string? ImageUrl { get; set; }   // Nullable property for image URL

        // Navigation property for OrderProducts
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        // You can add other properties here like Category, etc.
        public string? Category { get; set; }
    }
}