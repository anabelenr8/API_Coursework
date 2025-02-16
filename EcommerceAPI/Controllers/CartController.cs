using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public CartController(EcommerceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Cart>> GetCarts()
    {
        return Ok(_context.Carts.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<Cart> GetCart(int id)
    {
        var cart = _context.Carts.Find(id);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPost]
    public ActionResult<Cart> AddCart(Cart cart)
    {
        _context.Carts.Add(cart);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCart(int id, Cart updatedCart)
    {
        var cart = _context.Carts.Find(id);
        if (cart == null) return NotFound();

        cart.UserId = updatedCart.UserId;
        cart.ProductId = updatedCart.ProductId;
        cart.Quantity = updatedCart.Quantity;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCart(int id)
    {
        var cart = _context.Carts.Find(id);
        if (cart == null) return NotFound();

        _context.Carts.Remove(cart);
        _context.SaveChanges();
        return NoContent();
    }
}
