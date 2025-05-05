using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface IOrderProductService
    {
        Task<OrderProduct?> AddProductToOrder(OrderProductDTO orderProductDTO);
        Task<IEnumerable<OrderProductDTO>> GetProductsInOrder(int orderId);
    }
}
