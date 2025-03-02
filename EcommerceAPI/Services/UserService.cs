using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly EcommerceDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(EcommerceDbContext context, IEmailService emailService, ILogger<UserService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users...");
                return await _context.Users
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email ?? string.Empty,  
                        Role = u.Role
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return new List<UserDTO>();
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDTO model)
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

                // Send Welcome Email
                var subject = "Welcome to Ecommerce!";
                var body = $"Hello {model.Name}, your account has been successfully created.";
                var emailSent = await _emailService.SendEmailAsync(model.Email, subject, body);

                _logger.LogInformation(emailSent ? $"✅ Email sent to {model.Email}" : $"⚠️ Email sending failed for {model.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UserDTO updatedUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;
                user.Id = updatedUser.Id;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {id} updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {id} deleted.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return false;
            }
        }
    }
}
