using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Any logged in user
    public class UserProfileController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public UserProfileController(EcommerceDbContext context)
        {
            _context = context;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO profileDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            user.Name = profileDTO.Name;
            user.Email = profileDTO.Email;
            user.PhoneNumber = profileDTO.Phone;
            user.Address = profileDTO.Address;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully!" });
        }
    }
}
