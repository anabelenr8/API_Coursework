using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";

        // Navigation property: One user can have multiple orders
        public List<Order>? Orders { get; set; }
    }
}
