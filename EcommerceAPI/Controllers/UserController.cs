using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommerceAPI.Services;
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IEmailService _emailService;

        // Constructor
        public UserController(EcommerceDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ✅ GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDTO 
                { 
                    Id = u.Id, 
                    Name = u.Name ?? "",  // Ensure Name is never null
                    Email = u.Email ?? "", // Ensure Email is never null
                    Role = u.Role ?? ""   // Ensure Role is never null
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ GET SINGLE USER BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            var userDTO = new UserDTO 
            { 
                Id = user.Id, 
                Name = user.Name ?? "",  
                Email = user.Email ?? "",  
                Role = user.Role ?? ""  
            };

            return Ok(userDTO);
        }

        // ✅ REGISTER USER + SEND WELCOME EMAIL
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists!" });
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Welcome to Our Ecommerce API",
                "<h3>Your account has been successfully created!</h3><p>Thank you for registering.</p>"
            );

            return Ok(new { message = "User registered successfully!" });
        }

        // ✅ UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            user.Name = model.Name ?? user.Name;
            user.Email = model.Email ?? user.Email;
            user.Role = model.Role ?? user.Role;

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully!", user });
        }

        // ✅ DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = $"User with ID {id} not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

