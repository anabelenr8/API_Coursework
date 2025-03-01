using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceAPI.Services;
using EcommerceAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole<int>> _roleManager; 

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager, 
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials.");

            // ✅ Create claims safely
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // ✅ Fix: Ensure Email is not null
                new Claim(ClaimTypes.Role, user.Role ?? "User") // ✅ Default to "User" if null
            };

            var identity = new ClaimsIdentity(claims);
            var token = _jwtService.GenerateJwtToken(identity, user.Id.ToString(), user.Role ?? "User");

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists!" });

            // ✅ Assign Role - Default to "User" if not provided
            string role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { message = "Invalid role provided!" });

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Role = role
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, role);

            // ✅ Create claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // ✅ Ensure Email is not null
        new Claim(ClaimTypes.Role, user.Role ?? "User") // ✅ Default to "User" if null
    };

            var identity = new ClaimsIdentity(claims);
            var token = _jwtService.GenerateJwtToken(identity, user.Id.ToString(), user.Role ?? "User");

            return Ok(new { message = "User registered successfully!", token, role });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-data")]
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "This is protected admin data!" });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("user-data")]
        public IActionResult GetUserData()
        {
            return Ok(new { message = "This is accessible to both Users and Admins!" });
        }
    }
}
