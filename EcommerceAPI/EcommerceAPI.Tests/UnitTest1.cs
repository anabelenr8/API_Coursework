using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Controllers;
using EcommerceAPI.Models;
using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class OrderProductControllerTests
{
    private readonly OrderProductController _controller;
    private readonly EcommerceDbContext _context;

    public OrderProductControllerTests()
    {
        // ✅ Use a new InMemoryDatabase for each test
        var options = new DbContextOptionsBuilder<EcommerceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 🔹 Use unique DB name for each test
            .Options;

        _context = new EcommerceDbContext(options);

        // 🔹 Clear the database before each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // 🔹 Seed the database with test data
        _context.Orders.Add(new Order { Id = 1, UserId = 1 });
        _context.Products.Add(new Product { Id = 2, Name = "Phone" });
        _context.SaveChanges();

        _controller = new OrderProductController(_context);
    }

    [Fact]
    public void AddProductToOrder_ReturnsBadRequest_WhenOrderOrProductNotFound()
    {
        // Arrange
        var invalidOrderProduct = new OrderProduct { OrderId = 999, ProductId = 999, Quantity = 1 };

        // Act
        var result = _controller.AddProductToOrder(invalidOrderProduct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public void AddProductToOrder_ReturnsOk_WhenValidOrderProductIsAdded()
    {
        // Arrange
        var validOrderProduct = new OrderProduct { OrderId = 1, ProductId = 2, Quantity = 3 };

        // Act
        var result = _controller.AddProductToOrder(validOrderProduct);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }
}
