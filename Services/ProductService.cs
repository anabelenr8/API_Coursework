using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductService
{
    private readonly EcommerceDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(EcommerceDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET ALL PRODUCTS
    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        try
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description, // Include Description
                ImageUrl = p.ImageUrl,
                Category = p.Category   
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting products: {ex.Message}");
            throw;
        }
    }

    // GET PRODUCT BY ID
    public async Task<ProductDTO?> GetProductById(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description, // Include Description
                ImageUrl = product.ImageUrl,
                Category = product.Category
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting product by ID {id}: {ex.Message}");
            throw;
        }
    }

    // ADD PRODUCT
    public async Task<ProductDTO> AddProduct(ProductDTO productDTO)
    {
        try
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Stock = productDTO.Stock,
                Description = productDTO.Description,
                ImageUrl = productDTO.ImageUrl,
                Category = productDTO.Category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // Save to generate the ID

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Category = product.Category
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding product: {ex.Message}");
            throw;
        }
    }



    // UPDATE PRODUCT
    public async Task<bool> UpdateProduct(int id, ProductDTO productDTO)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = productDTO.Name;
            product.Price = productDTO.Price;
            product.Stock = productDTO.Stock;
            product.Description = productDTO.Description; // Update Description
            product.ImageUrl = productDTO.ImageUrl;
            product.Category = productDTO.Category;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating product: {ex.Message}");
            throw;
        }
    }

    // DELETE PRODUCT
    public async Task<bool> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting product: {ex.Message}");
            throw;
        }
    }
}
