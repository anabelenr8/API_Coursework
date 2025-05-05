using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET ALL PAYMENTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
        {
            try
            {
                var payments = await _paymentService.GetPayments();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching payments: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ADD PAYMENT
        [HttpPost]
        public async Task<ActionResult<PaymentDTO>> AddPayment(PaymentDTO paymentDTO)
        {
            try
            {
                var newPayment = await _paymentService.AddPayment(paymentDTO);
                return CreatedAtAction(nameof(GetPayments), new { id = newPayment.Id }, newPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding payment: {ex.Message}");
                return BadRequest($"Error processing payment: {ex.Message}");
            }
        }
        
    }
}
