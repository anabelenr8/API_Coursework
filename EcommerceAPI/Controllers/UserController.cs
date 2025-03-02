using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IEmailService _emailService;  // ✅ Inject Email Service
        private readonly ILogger<UserController> _logger; // ✅ Inject Logger

        public UserController(EcommerceDbContext context, IEmailService emailService, ILogger<UserController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // ✅ GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users...");
                var users = await _context.Users
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email ?? string.Empty,  // Prevent null issues
                        Role = u.Role
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ REGISTER USER WITH EMAIL NOTIFICATION
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            try
            {
                _logger.LogInformation($"Registering user: {model.Email}");

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ Send Welcome Email
                var subject = "Welcome to Ecommerce!";
                var body = $"Hello {model.Name}, your account has been successfully created.";
                var emailSent = await _emailService.SendEmailAsync(model.Email, subject, body);

                _logger.LogInformation(emailSent ? $"✅ Email sent to {model.Email}" : $"⚠️ Email sending failed for {model.Email}");

                return Ok(new { message = "User registered successfully! Email sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO updatedUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {id} updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {id} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
