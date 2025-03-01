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
    public class ProductController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public ProductController(EcommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL PRODUCTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock })
                .ToListAsync();
            return Ok(products);
        }

        // ✅ GET PRODUCT BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });

            return Ok(new ProductDTO { Id = product.Id, Name = product.Name, Price = product.Price, Stock = product.Stock });
        }

        // ✅ ADD A NEW PRODUCT
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Stock = productDTO.Stock
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTO);
        }
    }
}
