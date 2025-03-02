using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace EcommerceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            IJwtService jwtService,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<string?> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    _logger.LogWarning($"❌ Failed login attempt for {loginModel.Email}");
                    return null;
                }

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim("role", user.Role ?? "User")
                });

                return _jwtService.GenerateJwtToken(identity, user.Id.ToString(), user.Role ?? "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during login");
                return null;
            }
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return await Task.FromResult(Convert.ToBase64String(randomNumber));
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDTO model, string requestingUserRole)
        {
            try
            {
                // Prevent non-admin users from creating admin accounts
                if (model.Role == "Admin" && requestingUserRole != "Admin")
                {
                    _logger.LogWarning("❌ Unauthorized role assignment attempt!");
                    return false;
                }

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = model.Role ?? "User"  // Default to "User" if no role is provided
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("❌ User creation failed for {Email}", model.Email);
                    return false;
                }

                // Send confirmation email
                var subject = "Welcome to Ecommerce!";
                var body = $"Hello {model.Name}, your account has been successfully created.";
                await _emailService.SendEmailAsync(model.Email, subject, body);

                _logger.LogInformation("✅ User {Email} registered successfully with role {Role}", model.Email, user.Role);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during registration for {Email}", model.Email);
                return false;
            }
        }


    }
}
