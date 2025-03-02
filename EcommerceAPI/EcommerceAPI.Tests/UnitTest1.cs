using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Controllers;
using EcommerceAPI.Services;
using EcommerceAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class PaymentControllerTests
{
    private readonly PaymentController _controller;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<ILogger<PaymentController>> _mockLogger;

    public PaymentControllerTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _mockLogger = new Mock<ILogger<PaymentController>>();
        _controller = new PaymentController(_mockPaymentService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPayments_ReturnsListOfPayments()
    {
        _mockPaymentService.Setup(service => service.GetPayments())
            .ReturnsAsync(new List<PaymentDTO>
            {
            new PaymentDTO { Id = 1, UserId = 1, OrderId = 1, Amount = 100, PaymentMethod = "Credit Card", Status = "Completed" }
            });

        var result = await _controller.GetPayments();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // FIX: Ensure we check for List<T> instead of T[]
        var payments = (okResult.Value as IEnumerable<PaymentDTO>)?.ToList();
        Assert.NotNull(payments);
        Assert.Single(payments);

    }
}

// public class CartControllerTests
// {
//     private readonly CartController _controller;
//     private readonly Mock<ICartService> _mockCartService;

//     // public CartControllerTests()
    // {
    //     _mockCartService = new Mock<ICartService>();
    //     _controller = new CartController(_mockCartService.Object);
    // }

    // [Fact]
    // public async Task GetCarts_ReturnsListOfCarts()
    // {
    //     // Add test cart directly to the in-memory database
    //     var testCarts = new List<CartDTO>
    // {
    //     new CartDTO { Id = 1, UserId = 1, Items = new List<CartItemDTO> { new CartItemDTO { ProductId = 2, Quantity = 1 } } }
    // };

    //     _mockCartService.Setup(service => service.GetAllCartsAsync())
    //         .ReturnsAsync(testCarts);  // Seed the mock service with test data

    //     var result = await _controller.GetCarts();
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     var carts = Assert.IsType<List<CartDTO>>(okResult.Value);

    //     Assert.NotEmpty(carts);  // Change assertion from Assert.Single() to Assert.NotEmpty()
    // }

// }

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly Mock<IProductService> _mockProductService;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsListOfProducts()
    {
        _mockProductService.Setup(service => service.GetProducts())
            .ReturnsAsync(new List<ProductDTO> { new ProductDTO { Id = 1, Name = "Laptop" } });

        var result = await _controller.GetProducts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsType<List<ProductDTO>>(okResult.Value);
        Assert.Single(products);
    }
}

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<UserController>> _mockLogger;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_mockUserService.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterUser_ReturnsOk_WhenValidUserIsRegistered()
    {
        _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDTO>()))
            .ReturnsAsync(true);

        var newUser = new RegisterUserDTO { Name = "Ana", Email = "ana@example.com", Password = "Test123!" };

        var result = await _controller.Register(newUser);

        Assert.IsType<OkObjectResult>(result);
    }
}

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<IOrderService> _mockOrderService;

    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object);
    }

    [Fact]
    public async Task CreateOrder_ReturnsOk_WhenOrderIsCreated()
    {
        _mockOrderService.Setup(service => service.CreateOrderAsync(It.IsAny<OrderDTO>()))
            .ReturnsAsync(new OrderDTO { Id = 1, UserId = 1 });

        var order = new OrderDTO { UserId = 1, Items = new List<OrderItemDTO> { new OrderItemDTO { ProductId = 2, Quantity = 1 } } };

        var result = await _controller.CreateOrder(order);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}
