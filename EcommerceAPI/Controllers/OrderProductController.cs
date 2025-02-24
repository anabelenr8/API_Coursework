using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Linq;

[Route("api/orderproducts")]
[ApiController]
public class OrderProductController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public OrderProductController(EcommerceDbContext context)
    {
        _context = context;
    }

    // ✅ Add a Product to an Order
    [HttpPost]
    public ActionResult<OrderProduct> AddProductToOrder(OrderProduct orderProduct)
    {
        // Check if Order & Product Exist
        var order = _context.Orders.Find(orderProduct.OrderId);
        var product = _context.Products.Find(orderProduct.ProductId);

        if (order == null || product == null)
            return BadRequest(new { message = "Invalid OrderId or ProductId." });

        _context.OrderProducts.Add(orderProduct);
        _context.SaveChanges();

        return Ok(new { message = "Product added to order successfully!" });
    }

    // ✅ Get All Products in an Order
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
