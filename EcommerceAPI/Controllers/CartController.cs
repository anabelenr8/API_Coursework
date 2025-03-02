using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            try
            {
                var carts = await _cartService.GetCarts();
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            try
            {
                var cart = await _cartService.GetCartById(id);
                if (cart == null) return NotFound($"Cart with ID {id} not found.");
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            try
            {
                var newCart = await _cartService.AddCart(cartDto);
                return CreatedAtAction(nameof(GetCart), new { id = newCart.Id }, newCart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            try
            {
                var success = await _cartService.DeleteCart(id);
                if (!success) return NotFound($"Cart with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
