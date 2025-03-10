using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartDTO>> GetCarts();
        Task<CartDTO?> GetCartById(int id);
        Task<CartDTO> AddCart(CartDTO cartDto);
        Task<bool> DeleteCart(int id);
    }
}
