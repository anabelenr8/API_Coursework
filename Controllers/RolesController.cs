using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;


namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)

        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // assign user to a role
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok("Role created successfully.");
            }
            return BadRequest(result.Errors);
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }
            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return Ok("Role updated successfully.");
            }
            return BadRequest(result.Errors);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok("Role deleted successfully.");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("assign-role-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            // Step 1: Check if user exists
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("❌ User not found.");

            // Step 2: Check if role exists
            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
                return BadRequest($"❌ Role '{model.RoleName}' does not exist.");

            // Step 3: Get the user's current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Step 4: Remove the user from all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return StatusCode(500, $"❌ Failed to remove existing roles: {errors}");
            }

            // Step 5: Add the user to the new role
            var addResult = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return StatusCode(500, $"❌ Failed to assign new role: {errors}");
            }

            return Ok($"✅ Role set to '{model.RoleName}' for user '{user.Email}'.");
        }

    }
}