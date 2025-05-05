using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthModel model)
        {
            // Create a new user with the provided details
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                // Generate an email verification token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Create the verification link (URL encoding the token to avoid corruption)
                var encodedToken = System.Net.WebUtility.UrlEncode(token);
                var verificationLink = Url.Action("VerifyEmail", "Account",
                    new { userId = user.Id, token = encodedToken },
                    Request.Scheme);

                // DEBUG: Log the verification link
                Console.WriteLine($"Verification Link: {verificationLink}");

                // Send the verification email
                var emailSubject = "Email Verification";
                var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
                _emailService.SendEmail(user.Email, emailSubject, emailBody);

                // DEBUG: Include the token in the response temporarily for debugging
                return Ok(new
                {
                    Message = "User registered successfully. An email verification link has been sent.",
                    UserId = user.Id,
                    Token = token // REMOVE THIS in production for security
                });
            }

            // Return error messages if registration failed
            return BadRequest(result.Errors);
        }

        // Add an action to handle email verification
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Decode the token to fix encoding issues
            var decodedToken = Uri.UnescapeDataString(token);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok("Email verification successful.");
            }
            return BadRequest("Email verification failed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO model)
        {
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<User>>();

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    user = await userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return Unauthorized("Invalid login attempt.");
                    }

                    user = await userManager.FindByIdAsync(user.Id);
                    var roles = await userManager.GetRolesAsync(user);
                    Console.WriteLine($"✅ Roles fetched for {user.Email} upon login: {string.Join(", ", roles)}");

                    // WORKAROUND: Try removing the "User" role explicitly
                    if (roles.Contains("User"))
                    {
                        roles.Remove("User");
                    }

                    var token = GenerateJwtToken(user, roles);
                    Console.WriteLine($"✅ Token generated with roles: {string.Join(", ", roles)}");
                    return Ok(new { Token = token });
                }
                return Unauthorized("Invalid login attempt.");
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }
        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            // new Claim("displayName", user.Name ?? "Unknown"),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("displayName", user.UserName ?? user.UserName ?? "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var secretKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT Key is missing in appsettings.json!");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));
            var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: expires,
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}