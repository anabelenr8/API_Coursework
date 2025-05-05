using EcommerceAPI.DTOs;
using EcommerceAPI.Models;

namespace EcommerceAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<OrderDTO?> GetOrderById(int id);
        Task<OrderDTO> CreateOrderAsync(OrderDTO order);
        Task<Order> CreateOrder(OrderDTO orderDTO);
        Task<Order?> UpdateOrder(int id, OrderDTO updatedOrderDTO);
        Task<OrderProduct?> AddProductToOrder(OrderProduct orderProduct);

        Task<IEnumerable<Order>> GetOrdersAsync(string userId, bool isAdmin);


    }
}
