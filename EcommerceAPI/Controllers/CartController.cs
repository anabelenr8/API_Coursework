using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET ALL CARTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDTO>>> GetCarts()
        {
            var carts = await _context.Carts
                .Include(c => c.CartItems)
                .Select(c => new CartDTO
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Items = c.CartItems.Select(i => new CartItemDTO
                    {
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Ok(carts);
        }

        // GET CART BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null) return NotFound();

            var cartDTO = new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(i => new CartItemDTO
                {
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(cartDTO);
        }

        // ADD CART
        [HttpPost]
        public async Task<ActionResult<CartDTO>> AddCart(CartDTO cartDTO)
        {
            var cart = new Cart
            {
                UserId = cartDTO.UserId
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cartDTO);
        }

        // UPDATE CART
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, CartDTO updatedCart)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return NotFound();

            cart.UserId = updatedCart.UserId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE CART
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return NotFound();

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
