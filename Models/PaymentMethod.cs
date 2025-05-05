// Models/PaymentMethod.cs
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // Link to user

        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        public string Last4Digits { get; set; } = string.Empty; // Only last 4 digits stored

        [Required]
        public string ExpirationDate { get; set; } = string.Empty; // MM/YY format

        [Required]
        public string CardType { get; set; } = string.Empty; // e.g., Visa, MasterCard

        public bool IsDefault { get; set; } = false;
    }
}
