using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
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
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IEmailService emailService, ILogger<UserController> logger)
    {
        _userService = userService;
        _emailService = emailService;
        _logger = logger;
    }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var success = await _userService.RegisterUserAsync(model);
            if (!success) return StatusCode(500, "Internal server error.");
            return Ok(new { message = "User registered successfully! Email sent." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO updatedUser)
        {
            var success = await _userService.UpdateUserAsync(id, updatedUser);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
