using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public PaymentController(EcommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL PAYMENTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status
                })
                .ToListAsync();

            return Ok(payments);
        }

        // ✅ ADD PAYMENT
        [HttpPost]
        public async Task<ActionResult<PaymentDTO>> AddPayment(PaymentDTO paymentDTO)
        {
            var payment = new Payment
            {
                UserId = paymentDTO.UserId,
                OrderId = paymentDTO.OrderId,
                Amount = paymentDTO.Amount,
                PaymentMethod = paymentDTO.PaymentMethod,
                Status = paymentDTO.Status
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayments), new { id = payment.Id }, paymentDTO);
        }
    }
}