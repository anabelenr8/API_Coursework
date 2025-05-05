using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTOs
{
    public class UpdateEmailDTO
    {
        [Required]
        public string CurrentEmail { get; set; } = string.Empty;

        [Required]
        public string NewEmail { get; set; } = string.Empty;

        
    }

    public class UpdatePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
