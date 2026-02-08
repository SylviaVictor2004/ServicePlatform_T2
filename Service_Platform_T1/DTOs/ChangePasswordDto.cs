
using System.ComponentModel.DataAnnotations;

namespace Service_Platform_T1.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}