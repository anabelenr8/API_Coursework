using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using EcommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;


namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly EcommerceDbContext _context;

        public OrderController(IOrderService orderService, EcommerceDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        // GET ALL ORDERS
        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
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
                    Items = o.Items.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }


        // GET ORDER BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null) return NotFound($"Order with ID {id} not found.");
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // CREATE ORDER
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO orderDTO)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDTO);
                return Ok(new { orderId = createdOrder.Id }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // UPDATE ORDER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO updatedOrderDTO)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrder(id, updatedOrderDTO);
                if (updatedOrder == null) return NotFound($"Order with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDTO order)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                var newOrder = new Order
                {
                    UserId = userId, // Logged-in user's ID
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    ShippingAddress = order.ShippingAddress,
                    BillingAddress = order.BillingAddress,
                    PaymentMethod = order.PaymentMethod,
                    Items = order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(new { orderId = newOrder.Id, message = "Order placed" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error placing order: {ex}");
                return StatusCode(500, new { error = ex.Message });
            }

        }
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<List<OrderDTO>>> GetUserOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
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
                    Items = o.Items.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

    }
}
