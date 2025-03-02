using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using EcommerceAPI.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            try
            {
                _logger.LogInformation("🔹 Registering user: {Email}", model.Email);
                var success = await _authService.RegisterUserAsync(model);

                if (!success)
                {
                    _logger.LogWarning("❌ User registration failed for {Email}", model.Email);
                    return BadRequest(new { message = "User registration failed!" });
                }

                _logger.LogInformation("✅ User registered successfully: {Email}", model.Email);
                return Ok(new { message = "User registered successfully! An email has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Register method");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                _logger.LogInformation("🔹 Login attempt: {Email}", model.Email);
                var token = await _authService.LoginAsync(model);

                if (token == null)
                {
                    _logger.LogWarning("❌ Invalid login attempt: {Email}", model.Email);
                    return Unauthorized("Invalid credentials.");
                }

                _logger.LogInformation("✅ JWT Token Created for {Email}", model.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Login method");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
