namespace EcommerceAPI.Models
{
    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public string? Role { get; set; }
    }
}
