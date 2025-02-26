using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class OrderProductService : IOrderProductService
    {
        private readonly EcommerceDbContext _context;

        public OrderProductService(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<OrderProduct?> AddProductToOrder(OrderProduct orderProduct)
        {
            var order = await _context.Orders.FindAsync(orderProduct.OrderId);
            var product = await _context.Products.FindAsync(orderProduct.ProductId);

            if (order == null || product == null)
            {
                return null; // Handle this in the controller
            }

            var newOrderProduct = new OrderProduct
            {
                OrderId = orderProduct.OrderId,
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity
            };

            _context.OrderProducts.Add(newOrderProduct);
            await _context.SaveChangesAsync();

            return newOrderProduct;
        }
    }
}
