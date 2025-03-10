using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public class OrderProductService : IOrderProductService
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<OrderProductService> _logger;

        public OrderProductService(EcommerceDbContext context, ILogger<OrderProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Add a Product to an Order
        public async Task<OrderProduct?> AddProductToOrder(OrderProductDTO orderProductDTO)
        {
            try
            {
                var existingOrder = await _context.Orders.FindAsync(orderProductDTO.OrderId);
                var existingProduct = await _context.Products.FindAsync(orderProductDTO.ProductId);

                if (existingOrder == null || existingProduct == null)
                {
                    return null;
                }

                var newOrderProduct = new OrderProduct
                {
                    OrderId = orderProductDTO.OrderId,
                    ProductId = orderProductDTO.ProductId,
                    Quantity = orderProductDTO.Quantity,
                    Price = orderProductDTO.Price
                };

                _context.OrderProducts.Add(newOrderProduct);
                await _context.SaveChangesAsync();
                return newOrderProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product to order: {ex.Message}");
                throw;
            }
        }

        //Get All Products in an Order
        public async Task<IEnumerable<OrderProductDTO>> GetProductsInOrder(int orderId)
        {
            try
            {
                var orderProducts = await _context.OrderProducts
                    .Where(op => op.OrderId == orderId)
                    .ToListAsync();

                return orderProducts.Select(op => new OrderProductDTO
                {
                    OrderId = op.OrderId,
                    ProductId = op.ProductId,
                    Quantity = op.Quantity,
                    Price = op.Price
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving products in order: {ex.Message}");
                throw;
            }
        }
    }
}
