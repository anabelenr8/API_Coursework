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
    public class CartController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL CARTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDTO>>> GetCarts()
        {
            var carts = await _context.Carts
                .Include(c => c.CartItems)
                .Select(c => new CartDTO
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Items = c.CartItems.Select(i => new CartProductDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Ok(carts);
        }

        // ✅ GET CART BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null) return NotFound();

            return Ok(new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(i => new CartProductDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            });
        }
    }
}

