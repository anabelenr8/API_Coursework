using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models
{
    public class User : IdentityUser 
    {
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; } 
    
        public List<Order> Orders { get; set; } = new List<Order>();

        public override string? Email { get; set; } = string.Empty;

    }
}