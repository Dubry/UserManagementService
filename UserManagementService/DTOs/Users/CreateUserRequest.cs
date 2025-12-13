using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs.Users
{
    public class CreateUserRequest
    {
        [Required]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public string Language { get; set; }

        public string Culture { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
