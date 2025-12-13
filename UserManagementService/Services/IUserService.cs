using UserManagementService.DTOs.Users;
using UserManagementService.Models;

namespace UserManagementService.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize);
        Task<User> CreateUserAsync(User user, string password);
        Task UpdateUserAsync(Guid id, Action<User> updateAction);
        Task<int> GetUserCountAsync();
        Task DeleteUserAsync(Guid id);
        Task<bool> ValidatePasswordAsync(Guid userId, string password);
    }
}
