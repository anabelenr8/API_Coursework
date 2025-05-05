// Services/PaymentMethodService.cs
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class PaymentMethodService  : IPaymentMethod
{
    private readonly EcommerceDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaymentMethodService(EcommerceDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User not authenticated");
        }
        return userId;
    }

    public async Task<IEnumerable<PaymentMethodDTO>> GetUserPaymentMethodsAsync()
    {
        var userId = GetUserId();
        var methods = await _context.PaymentMethods
            .Where(pm => pm.UserId == userId)
            .Select(pm => new PaymentMethodDTO
            {
                Id = pm.Id,
                CardHolderName = pm.CardHolderName,
                Last4Digits = pm.Last4Digits,
                ExpirationDate = pm.ExpirationDate,
                CardType = pm.CardType
            })
            .ToListAsync();

        return methods;
    }

    public async Task AddPaymentMethodAsync(PaymentMethodDTO dto)
    {
        var userId = GetUserId();
        var newMethod = new PaymentMethod
        {
            UserId = userId,
            CardHolderName = dto.CardHolderName,
            Last4Digits = dto.Last4Digits,
            ExpirationDate = dto.ExpirationDate,
            CardType = dto.CardType
        };

        _context.PaymentMethods.Add(newMethod);
        await _context.SaveChangesAsync();
    }
}
