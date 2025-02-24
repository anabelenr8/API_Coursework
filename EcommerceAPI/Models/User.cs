using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Models
{
    public class User : IdentityUser<int> 
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}