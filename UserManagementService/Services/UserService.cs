using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Models;
using UserManagementService.Exceptions;
using UserManagementService.DTOs.Users;

namespace UserManagementService.Services
{
    public class UserService(AppDbContext context, IPasswordService passwordService) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly IPasswordService _passwordService = passwordService;

        public async Task<User> CreateUserAsync(User user, string password)
        {
            user.Password = _passwordService.HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id) ?? 
                throw new NotFoundException($"User with id '{id}' not found.");
            return user;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task UpdateUserAsync(Guid id, Action<User> updateAction)
        {
            var user = await _context.Users.FindAsync(id) ??
                throw new NotFoundException($"User with id '{id}' not found.");

            updateAction(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id) ??
                throw new NotFoundException($"User with id '{id}' not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null && _passwordService.VerifyPassword(password, user.Password);
        }
    }
}
