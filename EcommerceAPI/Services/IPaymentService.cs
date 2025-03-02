using EcommerceAPI.DTOs;
using EcommerceAPI.Models;

namespace EcommerceAPI.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetPayments();
        Task<Payment> AddPayment(PaymentDTO paymentDTO);
    }
}
