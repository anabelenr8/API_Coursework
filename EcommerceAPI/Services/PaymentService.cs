using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(EcommerceDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET ALL PAYMENTS
        public async Task<IEnumerable<PaymentDTO>> GetPayments()
        {
            try
            {
                var payments = await _context.Payments.ToListAsync();
                return payments.Select(p => new PaymentDTO
                {
                    UserId = p.UserId,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentMethod = p.PaymentMethod
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving payments: {ex.Message}");
                throw;
            }
        }

        // CREATE PAYMENT
        public async Task<Payment> AddPayment(PaymentDTO paymentDTO)
        {
            try
            {
                // Validate that the order exists before creating a payment
                var orderExists = await _context.Orders.AnyAsync(o => o.Id == paymentDTO.OrderId);
                if (!orderExists)
                {
                    throw new Exception("Order does not exist.");
                }

                var payment = new Payment
                {
                    UserId = paymentDTO.UserId,
                    OrderId = paymentDTO.OrderId,
                    Amount = paymentDTO.Amount,
                    Status = paymentDTO.Status,
                    PaymentMethod = paymentDTO.PaymentMethod
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating payment: {ex.Message}");
                throw;
            }
        }
    }
}
