namespace EcommerceAPI.DTOs
{
    public class UserDTO
    {
        public required string Id { get; set; }  
        public required string Name { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
    }
}

