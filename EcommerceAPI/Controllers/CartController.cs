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

        // Get all carts
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
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Get a single cart by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            try
            {
                var cart = await _cartService.GetCartById(id);
                if (cart == null) 
                    return NotFound(new { message = $"Cart with ID {id} not found." });

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Add a new cart (ID is automatically generated)
        [HttpPost]

        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            try
            {
                if (cartDto == null || string.IsNullOrEmpty(cartDto.UserId) || cartDto.Items == null || cartDto.Items.Count == 0)
                    return BadRequest(new { message = "Invalid cart data. Must include a valid user ID and at least one item." });

                var newCart = await _cartService.AddCart(cartDto);

                return CreatedAtAction(nameof(GetCart), new { id = newCart.Id }, newCart); // Now returns generated `Id`
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }



        // Delete a cart by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            try
            {
                var success = await _cartService.DeleteCart(id);
                if (!success) 
                    return NotFound(new { message = $"Cart with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
