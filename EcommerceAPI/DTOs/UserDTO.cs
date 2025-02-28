namespace EcommerceAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;  // Fix: Required added
        public required string Email { get; set; } = string.Empty;  // Fix: Required added
        public required string Role { get; set; } = string.Empty;  // Fix: Required added
    }
}
