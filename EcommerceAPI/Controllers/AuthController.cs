using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceAPI.Services;
using System.Threading.Tasks;
using EcommerceAPI.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EcommerceAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                _logger.LogInformation("🔹 Registering user: {Email}", model.Email);

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("❌ User already exists: {Email}", model.Email);
                    return BadRequest(new { message = "User already exists!" });
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Role = "User"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("❌ User creation failed: {Errors}", result.Errors);
                    return BadRequest(result.Errors);
                }

                // ✅ Send Welcome Email
                string subject = "Welcome to Ecommerce!";
                string body = $"<h3>Hi {user.Name},</h3><p>Your account has been successfully created.</p>";
                await _emailService.SendEmailAsync(user.Email, subject, body);

                _logger.LogInformation("✅ User registered successfully: {Email}", user.Email);
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("❌ Invalid login attempt: {Email}", model.Email);
                    return Unauthorized("Invalid credentials.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("❌ Invalid login credentials: {Email}", model.Email);
                    return Unauthorized("Invalid credentials.");
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("✅ JWT Token Created for {Email}", user.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Login method");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("role", user.Role ?? "User")
            };

            _logger.LogInformation("🔹 Claims in JWT: {Claims}", string.Join(", ", claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "1"));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            ));
        }
    }
}

