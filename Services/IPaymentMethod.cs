
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services
{
    public interface IPaymentMethod
    {
        Task<IEnumerable<PaymentMethodDTO>> GetUserPaymentMethodsAsync();
        Task AddPaymentMethodAsync(PaymentMethodDTO dto);
    }
}
