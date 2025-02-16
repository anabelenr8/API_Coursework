using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public OrderController(EcommerceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        return Ok(_context.Orders.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetOrder(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public ActionResult<Order> AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, Order updatedOrder)
    {
        var order = _context.Orders.Find(id);
        if (order == null) return NotFound();

        order.UserId = updatedOrder.UserId;
        order.OrderDate = updatedOrder.OrderDate;
        order.TotalAmount = updatedOrder.TotalAmount;
        order.Status = updatedOrder.Status;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        _context.SaveChanges();
        return NoContent();
    }
}
