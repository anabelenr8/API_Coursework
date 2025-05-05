// Controllers/PaymentMethodController.cs
using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethod _paymentMethodService;
    private readonly EcommerceDbContext _context;

    public PaymentMethodController(IPaymentMethod paymentMethodService, EcommerceDbContext context)
    {
        _paymentMethodService = paymentMethodService;
         _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var methods = await _paymentMethodService.GetUserPaymentMethodsAsync();
        return Ok(methods);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentMethodDTO dto)
    {
        await _paymentMethodService.AddPaymentMethodAsync(dto);
        return Ok(new { message = "Payment method saved!" });
    }

    [Authorize]
    [HttpGet("default")]
    public async Task<IActionResult> GetDefaultPaymentMethod()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsDefault);

        if (method == null)
            return NotFound("No default payment method found.");

        return Ok(method);
    }

    [Authorize]
    [HttpPut("default/{id}")]
    public async Task<IActionResult> SetDefault(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var methods = await _context.PaymentMethods
            .Where(pm => pm.UserId == userId)
            .ToListAsync();

        if (!methods.Any())
            return NotFound("No payment methods found for this user.");

        foreach (var method in methods)
        {
            method.IsDefault = method.Id == id;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Default payment method updated." });
    }

}
