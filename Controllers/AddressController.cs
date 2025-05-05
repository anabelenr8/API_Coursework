using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly AddressService _addressService;
    private readonly EcommerceDbContext _context;

    public AddressController(AddressService addressService, EcommerceDbContext context)
    {
        _addressService = addressService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        var addresses = await _addressService.GetUserAddressesAsync();
        return Ok(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] AddressDTO dto)
    {
        await _addressService.AddAddressAsync(dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _addressService.DeleteAddressAsync(id);
        return Ok();
    }

    [Authorize]
    [HttpPost("default/{id}")]
    public IActionResult SetDefaultAddress(int id)
    {
        // Sample logic (adapt to your structure)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAddresses = _context.Addresses.Where(a => a.UserId == userId);

        foreach (var address in userAddresses)
        {
            address.IsDefault = address.Id == id;
        }

        _context.SaveChanges();
        return Ok();
    }

    [Authorize]
    [HttpGet("default")]
    public IActionResult GetDefaultAddress()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var defaultAddress = _context.Addresses
            .FirstOrDefault(a => a.UserId == userId && a.IsDefault == true);

        if (defaultAddress == null)
            return NotFound("No default address found.");

        return Ok(defaultAddress);
    }



}
