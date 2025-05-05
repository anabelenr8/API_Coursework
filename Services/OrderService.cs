using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(EcommerceDbContext context, ILogger<OrderService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await _context.Orders
                    .Include(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                    .Include(o => o.User)
                    .ToListAsync();
            }
            else
            {
                return await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                    .ToListAsync();
            }
        }



        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO)
        {
            var order = new Order
            {
                UserId = orderDTO.UserId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderProducts = orderDTO.Items.Select(item => new OrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };
            

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Ensure ID is generated

            // Return the generated Order ID
            return new OrderDTO
            {
                Id = order.Id,  // Include the newly generated Order ID
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderProducts.Select(op => new OrderItemDTO
                {
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                }).ToList()
            };
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



        // ✅ GET ALL ORDERS
        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return new List<OrderDTO>();
                }

                var orders = await _context.Orders
                    .Where(o => o.UserId == userId) // ✅ FILTER by user here
                    .Include(o => o.OrderProducts)
                    .Select(o => new OrderDTO
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        ShippingAddress = o.ShippingAddress,
                        BillingAddress = o.BillingAddress,
                        PaymentMethod = o.PaymentMethod,
                        Items = o.OrderProducts.Select(op => new OrderItemDTO
                        {
                            ProductId = op.ProductId,
                            Quantity = op.Quantity,
                            Price = op.Price
                        }).ToList()
                    })
                    .ToListAsync();

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving orders: {ex.Message}");
                throw;
            }
        }

        // ✅ GET ORDER BY ID
        public async Task<OrderDTO?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Id == id)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,       // 👈 Make sure this is here
                    BillingAddress = o.BillingAddress,         // 👈 And this
                    PaymentMethod = o.PaymentMethod,           // 👈 And this
                    Items = o.Items.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }


        // ✅ CREATE ORDER
        public async Task<Order> CreateOrder(OrderDTO orderDTO)
        {
            try
            {
                var order = new Order
                {
                    UserId = orderDTO.UserId,
                    OrderProducts = orderDTO.Items.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order: {ex.Message}");
                throw;
            }
        }

        // ✅ UPDATE ORDER
        public async Task<Order?> UpdateOrder(int id, OrderDTO updatedOrderDTO)
        {
            try
            {
                var order = await _context.Orders.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.Id == id);
                if (order == null) return null;

                order.UserId = updatedOrderDTO.UserId;
                order.OrderDate = updatedOrderDTO.OrderDate;
                order.TotalAmount = updatedOrderDTO.TotalAmount;
                order.Status = updatedOrderDTO.Status;

                // ✅ Update Order Items
                order.OrderProducts.Clear();
                foreach (var item in updatedOrderDTO.Items)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }

                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order {id}: {ex.Message}");
                throw;
            }
        }
    }
}

