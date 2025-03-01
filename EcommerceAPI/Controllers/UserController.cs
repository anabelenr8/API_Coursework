using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
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
        private readonly IEmailService _emailService;  // Inject Email Service

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
                    Name = u.Name,
                    Email = u.Email ?? string.Empty,  //Prevent null issues
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ REGISTER USER WITH EMAIL NOTIFICATION
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // ✅ Debug: Check if this line runs
            Console.WriteLine($"Sending email to {model.Email}...");

            var subject = "Welcome to Ecommerce!";
            var body = $"Hello {model.Name}, your account has been created!";
            var emailSent = await _emailService.SendEmailAsync(model.Email, subject, body);

            // ✅ Debug: Confirm if email was sent
            Console.WriteLine(emailSent ? " Email sent successfully!" : "Email failed!");

            return Ok(new { message = "User registered successfully! Email sent." });
        }


        // ✅ UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
