using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class CartService : ICartService
    {
        private readonly EcommerceDbContext _context;

        public CartService(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartDTO>> GetCarts()
        {
            try
            {
                var carts = await _context.Carts.Include(c => c.CartItems).ToListAsync();
                return carts.Select(c => new CartDTO
                {
                    Id = c.Id,
                    UserId = c.UserId
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCarts: {ex.Message}");
                return new List<CartDTO>();
            }
        }

        public async Task<CartDTO?> GetCartById(int id)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(id);
                if (cart == null) return null;

                return new CartDTO
                {
                    Id = cart.Id,
                    UserId = cart.UserId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCartById: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CartDTO>> GetAllCartsAsync()
        {
            Console.WriteLine($"Carts Found: {_context.Carts.Count()}");

            return await _context.Carts
                .Include(c => c.CartItems)
                .Select(c => new CartDTO
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Items = c.CartItems.Select(ci => new CartItemDTO
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<CartDTO> AddCart(CartDTO cartDto)
        {
            try
            {
                var newCart = new Cart
                {
                    UserId = cartDto.UserId
                };

                _context.Carts.Add(newCart);
                await _context.SaveChangesAsync();

                return new CartDTO { Id = newCart.Id, UserId = newCart.UserId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddCart: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsAsync()
        {
            return await _context.Payments
                .Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status
                }).ToListAsync();
        }

        public async Task<bool> DeleteCart(int id)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(id);
                if (cart == null) return false;

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteCart: {ex.Message}");
                return false;
            }
        }
    }
}
