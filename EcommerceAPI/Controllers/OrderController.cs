using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using System.Collections.Generic;

[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderProductService _orderProductService;
    private readonly EcommerceDbContext _context;

    public OrderController(IOrderProductService orderProductService, EcommerceDbContext context)
    {
        _orderProductService = orderProductService;
        _context = context; // Now _context is properly initialized
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToOrder([FromBody] OrderProduct orderProduct)
    {
        var result = await _orderProductService.AddProductToOrder(orderProduct);

        if (result == null)
        {
            return BadRequest(new { message = "Invalid OrderId or ProductId." });
        }

        return Ok(new { message = "Product added to order successfully!", orderProduct = result });
    }

    // Get All Orders
    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        return Ok(_context.Orders.ToList());
    }

    // Get Order Details by OrderId
    [HttpGet("{orderId}")]
    public ActionResult<Order> GetOrder(int orderId)
    {
        var order = _context.Orders.Find(orderId);
        if (order == null) return NotFound(new { message = "Order not found" });
        return Ok(order);
    }

    // Get Orders for a Specific User
    [HttpGet("user/{userId}")]
    public ActionResult<IEnumerable<Order>> GetOrdersByUser(int userId)
    {
        var userOrders = _context.Orders.Where(o => o.UserId == userId).ToList();
        if (!userOrders.Any()) return NotFound(new { message = "No orders found for this user" });

        return Ok(userOrders);
    }

    // Create Order
    [HttpPost("create")]
    public ActionResult<Order> AddOrder(Order order)
    {
        var user = _context.Users.Find(order.UserId);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid UserId. User does not exist." });
        }

        order.User = user;
        _context.Orders.Add(order);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
    }

    // Update Order
    [HttpPut("{orderId}")]
    public IActionResult UpdateOrder(int orderId, Order updatedOrder)
    {
        var order = _context.Orders.Find(orderId);
        if (order == null) return NotFound(new { message = "Order not found" });

        order.UserId = updatedOrder.UserId;
        order.OrderDate = updatedOrder.OrderDate;
        order.TotalAmount = updatedOrder.TotalAmount;
        order.Status = updatedOrder.Status;

        _context.SaveChanges();
        return NoContent();
    }

    // Delete Order
    [HttpDelete("{orderId}")]
    public IActionResult DeleteOrder(int orderId)
    {
        var order = _context.Orders.Find(orderId);
        if (order == null) return NotFound(new { message = "Order not found" });

        _context.Orders.Remove(order);
        _context.SaveChanges();
        return NoContent();
    }
}

