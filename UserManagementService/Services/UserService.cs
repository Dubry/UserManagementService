using Microsoft.EntityFrameworkCore;
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

        public async Task<User> CreateUserAsync(User user, string password)
        {
            user.Password = _passwordService.HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<bool> UpdateUserAsync(Guid id, Action<User> updateAction)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            updateAction(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null && _passwordService.VerifyPassword(password, user.Password);
        }
    }
}
