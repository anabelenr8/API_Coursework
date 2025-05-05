using System.Security.Claims;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All users must be logged in
    public class UserSecurityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserSecurityController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Check current email matches
            if (!string.Equals(user.Email, dto.CurrentEmail, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Current email does not match our records." });
            }

            // Generate token for email change
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NewEmail);
            var emailChangeResult = await _userManager.ChangeEmailAsync(user, dto.NewEmail, token);

            if (!emailChangeResult.Succeeded)
            {
                return BadRequest(emailChangeResult.Errors);
            }

            // Update username and normalized fields
            user.UserName = dto.NewEmail;
            user.NormalizedUserName = dto.NewEmail.ToUpper();
            user.NormalizedEmail = dto.NewEmail.ToUpper();

            // Save changes
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok(new { message = "Email updated successfully!" });
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password updated successfully!" });
        }
    }
}
