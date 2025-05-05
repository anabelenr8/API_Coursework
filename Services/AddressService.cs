using System.Security.Claims;
using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore;

public class AddressService
{
    private readonly EcommerceDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddressService(EcommerceDbContext context, IHttpContextAccessor httpContextAccessor)
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

    public async Task<IEnumerable<AddressDTO>> GetUserAddressesAsync()
    {
        var userId = GetUserId();
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .Select(a => new AddressDTO
            {
                Id = a.Id,
                FullName = a.FullName,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country,
                IsDefault = a.IsDefault
            })
            .ToListAsync();
    }

    public async Task AddAddressAsync(AddressDTO dto)
    {
        var userId = GetUserId();
        var address = new Address
        {
            UserId = userId,
            FullName = dto.FullName,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            IsDefault = dto.IsDefault
        };
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAddressAsync(int id)
    {
        var userId = GetUserId();
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address != null)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
