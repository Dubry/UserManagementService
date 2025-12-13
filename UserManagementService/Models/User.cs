using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Required]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public string Language {  get; set; }

        public string Culture { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}
