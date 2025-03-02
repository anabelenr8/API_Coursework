using EcommerceAPI.Services;
using EcommerceAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving products", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null) return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving product", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDTO)
        {
            try
            {
                await _productService.AddProduct(productDTO);
                return CreatedAtAction(nameof(GetProduct), new { id = productDTO.Id }, productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding product", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            try
            {
                if (id != productDTO.Id)
                    return BadRequest(new { message = "ID mismatch" });

                await _productService.UpdateProduct(productDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating product", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting product", error = ex.Message });
            }
        }
    }
}
