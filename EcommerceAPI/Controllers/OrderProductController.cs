using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Services;
using EcommerceAPI.DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/orderproducts")]
    [ApiController]
    public class OrderProductController : ControllerBase
    {
        private readonly IOrderProductService _orderProductService;
        private readonly ILogger<OrderProductController> _logger;

        public OrderProductController(IOrderProductService orderProductService, ILogger<OrderProductController> logger)
        {
            _orderProductService = orderProductService;
            _logger = logger;
        }

        // Add a Product to an Order
        [HttpPost]
        public async Task<ActionResult> AddProductToOrder([FromBody] OrderProductDTO orderProductDTO)
        {
            try
            {
                var newOrderProduct = await _orderProductService.AddProductToOrder(orderProductDTO);
                if (newOrderProduct == null)
                    return BadRequest(new { message = "Invalid OrderId or ProductId." });

                return Ok(new { message = "Product added to order successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product to order: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get All Products in an Order
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderProductDTO>>> GetProductsInOrder(int orderId)
        {
            try
            {
                var orderProducts = await _orderProductService.GetProductsInOrder(orderId);
                if (orderProducts == null || !orderProducts.Any())
                    return NotFound(new { message = "No products found for this order" });

                return Ok(orderProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching products in order: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
