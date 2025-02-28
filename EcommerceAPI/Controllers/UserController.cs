using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IEmailService _emailService;

        // ✅ Combined Constructor (Injects both DbContext & Email Service)
        public UserController(EcommerceDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ✅ GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // ✅ GET SINGLE USER BY ID
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

        // ✅ REGISTER USER + SEND WELCOME EMAIL
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            Console.WriteLine($"Registering user: {model.Email}");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                Console.WriteLine("User already exists.");
                return BadRequest(new { message = "User already exists!" });
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Attempting to send email to {user.Email}...");

            await _emailService.SendEmailAsync(
                user.Email,
                "Welcome to Our Ecommerce API",
                "<h3>Your account has been successfully created!</h3><p>Thank you for registering.</p>"
            );

            Console.WriteLine("Email function executed.");

            return Ok(new { message = "User registered successfully! A welcome email has been sent." });
        }

        // ✅ CREATE A NEW USER (Manual Entry)
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

        // ✅ UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
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
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully", user = existingUser });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The user was modified by another process. Refresh and try again." });
            }
        }

        // ✅ DELETE USER
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
}

