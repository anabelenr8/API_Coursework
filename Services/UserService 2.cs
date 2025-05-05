using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services;

public class UserService : IUserService
{
    private readonly EcommerceDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(EcommerceDbContext context, ILogger<UserService> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ✅ GET USERS with accurate roles from Identity
    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all users with roles...");

            var users = await _userManager.Users.ToListAsync();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "NoRole"
                });
            }

            return userDTOs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching users.");
            return new List<UserDTO>();
        }
    }

    // ✅ UPDATE USER (name, email, and roles properly via UserManager)
    public async Task<bool> UpdateUserAsync(string id, UserDTO updatedUser)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User {id} not found for update.");
                return false;
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = updatedUser.Role;

            // Remove roles the user currently has but shouldn't
            foreach (var role in currentRoles.Where(r => r != newRole))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, role);
                if (!removeResult.Succeeded)
                {
                    _logger.LogWarning($"Failed to remove role '{role}' from user {id}: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                    return false;
                }
            }

            // Add the new role if the user doesn't have it
            if (!string.IsNullOrWhiteSpace(newRole) && !currentRoles.Contains(newRole))
            {
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    _logger.LogWarning($"Failed to add role '{newRole}' to user {id}: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                    return false;
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning($"Failed to update user {id}: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                return false;
            }

            _logger.LogInformation($"User {id} updated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user {id}");
            return false;
        }
    }

    // ✅ DELETE USER properly via UserManager
    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User {id} not found for deletion.");
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed to delete user {id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }

            _logger.LogInformation($"User {id} deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user {id}");
            return false;
        }
    }
    
    public async Task<bool> UpdateUserRoleAsync(Guid id, string newRole)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return false;

        var addResult = await _userManager.AddToRoleAsync(user, newRole);
        return addResult.Succeeded;
    }

}
