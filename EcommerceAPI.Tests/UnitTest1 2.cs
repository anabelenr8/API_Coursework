using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Controllers;
using EcommerceAPI.Services;
using EcommerceAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EcommerceAPI.Data;

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
            new PaymentDTO { UserId = "asgdtdtydyd-bbgyfy0986", OrderId = 1, Amount = 100, PaymentMethod = "Credit Card", Status = "Completed" }
            });

        var result = await _controller.GetPayments();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // FIX: Ensure we check for List<T> instead of T[]
        var payments = (okResult.Value as IEnumerable<PaymentDTO>)?.ToList();
        Assert.NotNull(payments);
        Assert.Single(payments);

    }
}

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<EcommerceDbContext> _mockContext;
    public OrderControllerTests()
    {
         _mockOrderService = new Mock<IOrderService>();
        _mockContext = new Mock<EcommerceDbContext>();
        _controller = new OrderController(_mockOrderService.Object, _mockContext.Object);
    }

    [Fact]
    public async Task CreateOrder_ReturnsOk_WhenOrderIsCreated()
    {
        _mockOrderService.Setup(service => service.CreateOrderAsync(It.IsAny<OrderDTO>()))
            .ReturnsAsync(new OrderDTO { Id = 1, UserId = "asgdtdtydyd-bbgyfy0986" });

        var order = new OrderDTO { UserId = "asgdtdtydyd-bbgyfy0986", Items = new List<OrderItemDTO> { new OrderItemDTO { ProductId = 2, Quantity = 1 } } };

        var result = await _controller.CreateOrder(order);

        Assert.IsType<CreatedAtActionResult>(result);
    }
    
}
