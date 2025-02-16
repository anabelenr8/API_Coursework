using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static List<Order> orders = new List<Order>();

        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            orders.Add(order);
            return Ok(order);
        }
    }
}
