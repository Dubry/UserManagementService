using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Models;
using UserManagementService.Services;
using Xunit;

namespace UserManagementService.Tests.Services
{
    public class UserServiceTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldPersistUser()
        {
            var db = CreateDbContext();
            var passwordService = new PasswordService();
            var service = new UserService(db, passwordService);

            var user = new User { UserName = "jdoe", Email = "jdoe@test.com" };

            var result = await service.CreateUserAsync(user, "password");

            result.Id.Should().NotBeEmpty();
            db.Users.Count().Should().Be(1);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldPersistUser()
        {
            var db = CreateDbContext();
            var passwordService = new PasswordService();
            var service = new UserService(db, passwordService);

            var user = new User { UserName = "jdoe", Email = "jdoe@test.com" };

            var result = await service.CreateUserAsync(user, "password");

            result.Id.Should().NotBeEmpty();
            db.Users.Count().Should().Be(1);
        }
    }
}
