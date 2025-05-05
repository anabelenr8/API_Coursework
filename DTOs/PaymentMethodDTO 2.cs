// DTOs/PaymentMethodDTO.cs
namespace EcommerceAPI.DTOs
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string Last4Digits { get; set; } = string.Empty;
        public string ExpirationDate { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
    }
}
