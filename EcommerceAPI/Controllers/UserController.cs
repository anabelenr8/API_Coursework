using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public UserController(EcommerceDbContext context)
    {
        _context = context;
    }

    // GET ALL USERS
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // GET SINGLE USER BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }
        return Ok(user);
    }

    // CREATE A NEW USER
    [HttpPost]
    public async Task<ActionResult<User>> AddUser(User user)
    {
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
        {
            return BadRequest(new { message = "Email and Username are required!" });
        }

        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest(new { message = "Email already in use!" });
        }

        // Assign default role if not set
        user.Role ??= "User";

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // UPDATE USER
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        var existingUser = _context.Users.Find(id);
        if (existingUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Update user properties
        existingUser.UserName = updatedUser.UserName;
        existingUser.Email = updatedUser.Email;
        existingUser.Role = updatedUser.Role;

        try
        {
            _context.SaveChanges();
            return Ok(new { message = "User updated successfully", user = existingUser });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The user was modified by another process. Refresh and try again." });
        }
    }


    // DELETE USER
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
