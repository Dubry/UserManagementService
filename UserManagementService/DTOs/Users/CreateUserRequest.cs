using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs.Users
{
    public class CreateUserRequest
    {
        [Required]
        public string UserName { get; set; } = default!;

        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? MobileNumber { get; set; }

        public string? Language { get; set; }

        public string? Culture { get; set; }

        [Required]
        public string Password { get; set; } = default!;
    }
}
