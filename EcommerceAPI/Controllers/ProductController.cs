using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Linq;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public ProductController(EcommerceDbContext context)
    {
        _context = context;
    }

    // Get All Products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        return Ok(_context.Products.ToList());
    }

    // Get a Single Product by ID
    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    // Add a New Product
    [HttpPost]
    public ActionResult<Product> AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
