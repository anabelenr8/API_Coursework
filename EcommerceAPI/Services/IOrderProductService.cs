using EcommerceAPI.Models;

namespace EcommerceAPI.Services
{
    public interface IOrderProductService
    {
        Task<OrderProduct?> AddProductToOrder(OrderProduct orderProduct);
    }
}
