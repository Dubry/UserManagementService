using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs.Users
{
    public class ValidatePasswordRequest
    {
        [Required]
        public string Password { get; set; }
    }
}
