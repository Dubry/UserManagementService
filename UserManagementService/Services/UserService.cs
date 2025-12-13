using UserManagementService.Data;
using UserManagementService.Models;

namespace UserManagementService.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public UserService(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public User CreateUser(User user, string password)
        {
            user.Password = _passwordService.HashPassword(password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public bool ValidatePassword(Guid userId, string password)
        {
            var user = _context.Users.Find(userId);
            return user != null && _passwordService.VerifyPassword(password, user.Password);
        }
    }
}
