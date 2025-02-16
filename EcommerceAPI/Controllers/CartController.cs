using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private static List<Cart> cartItems = new List<Cart>();

        [HttpGet]
        public IActionResult GetCart()
        {
            return Ok(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(Cart item)
        {
            cartItems.Add(item);
            return Ok(item);
        }
    }
}
