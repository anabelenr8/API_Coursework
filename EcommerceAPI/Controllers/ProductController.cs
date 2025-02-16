using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static List<Product> products = new List<Product>();

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(products);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            products.Add(product);
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }
    }
}
