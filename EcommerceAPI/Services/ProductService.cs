using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    private readonly EcommerceDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(EcommerceDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        try
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(p => new ProductDTO { Id = p.Id, Name = p.Name }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting products: {ex.Message}");
            throw;
        }
    }

    public async Task<ProductDTO> GetProductById(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null!;

            return new ProductDTO { Id = product.Id, Name = product.Name };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting product by ID {id}: {ex.Message}");
            throw;
        }
    }

    public async Task AddProduct(ProductDTO productDTO)
    {
        try
        {
            var product = new Product { Name = productDTO.Name };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding product: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateProduct(ProductDTO productDTO)
    {
        try
        {
            var product = await _context.Products.FindAsync(productDTO.Id);
            if (product == null) return;

            product.Name = productDTO.Name;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating product: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting product: {ex.Message}");
            throw;
        }
    }
}
