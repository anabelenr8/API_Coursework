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
    public class OrderController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public OrderController(EcommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL ORDERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    Items = o.OrderProducts.Select(op => new OrderItemDTO // ✅ FIXED: Renamed
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // ✅ GET ORDER BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return Ok(new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderProducts.Select(op => new OrderItemDTO // ✅ FIXED: Renamed
                {
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                }).ToList()
            });
        }

        // ✅ CREATE ORDER
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO orderDTO)
        {
            var order = new Order
            {
                UserId = orderDTO.UserId,
                OrderProducts = orderDTO.Items.Select(op => new OrderProduct // ✅ FIXED: Renamed
                {
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // ✅ UPDATE ORDER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO updatedOrderDTO)
        {
            var order = await _context.Orders.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            order.UserId = updatedOrderDTO.UserId;
            order.OrderDate = updatedOrderDTO.OrderDate;
            order.TotalAmount = updatedOrderDTO.TotalAmount;
            order.Status = updatedOrderDTO.Status;

            // ✅ Update Order Items
            order.OrderProducts.Clear();
            foreach (var item in updatedOrderDTO.Items) // ✅ FIXED: Renamed
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
