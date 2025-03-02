namespace EcommerceAPI.DTOs
{
    public class UserDTO
    {
        public required string Name { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public int Id { get; set; }
        public required string Role { get; set; } = string.Empty;
    }
}
