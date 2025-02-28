using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models
{
    public class User : IdentityUser<int> 
    {
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public string Role { get; set; } = "User";

        public List<Order> Orders { get; set; } = new List<Order>();

        public override string? Email { get; set; } = string.Empty;
    }
}