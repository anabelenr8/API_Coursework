using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/orderproducts")]
[ApiController]
public class OrderProductController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public OrderProductController(EcommerceDbContext context)
    {
        _context = context;
    }

    // Add a Product to an Order
    [HttpPost]
    public ActionResult<OrderProduct> AddProductToOrder(OrderProduct orderProduct)
    {
        var existingOrder = _context.Orders.AsNoTracking().FirstOrDefault(o => o.Id == orderProduct.OrderId);
        var existingProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == orderProduct.ProductId);

        if (existingOrder == null || existingProduct == null)
        {
            return BadRequest(new { message = "Invalid OrderId or ProductId." });
        }

        // Correct way: Only set OrderId & ProductId, no full Order/Product objects
        var newOrderProduct = new OrderProduct
        {
            OrderId = orderProduct.OrderId,
            ProductId = orderProduct.ProductId,
            Quantity = orderProduct.Quantity
        };

        _context.OrderProducts.Add(newOrderProduct);
        _context.SaveChanges();

        return Ok(new { message = "Product added to order successfully!" });
    }



    // Get All Products in an Order
    [HttpGet("order/{orderId}")]
    public ActionResult<IEnumerable<OrderProduct>> GetProductsInOrder(int orderId)
    {
        var orderProducts = _context.OrderProducts
            .Where(op => op.OrderId == orderId)
            .ToList();

        if (!orderProducts.Any())
            return NotFound(new { message = "No products found for this order" });

        return Ok(orderProducts);
    }
}