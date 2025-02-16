using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private static List<Payment> payments = new List<Payment>();

        [HttpGet]
        public IActionResult GetPayments()
        {
            return Ok(payments);
        }

        [HttpPost]
        public IActionResult MakePayment(Payment payment)
        {
            payments.Add(payment);
            return Ok(payment);
        }
    }
}
