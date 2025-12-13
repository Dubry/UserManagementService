using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UserManagementService.Data;
using UserManagementService.Models;
using UserManagementService.Services;

namespace UserManagementService.IntegrationTests
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureTestServices(services =>
            {
                
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IPasswordService, PasswordService>();
                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to seed the database
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted(); 
                db.Database.EnsureCreated();

                SeedApiClients(db);
                SeedUsers(db);
            });
        }

        private static void SeedApiClients(AppDbContext db)
        {
            if (db.ApiClients.Any())
                return;

            db.ApiClients.Add(new ApiClient
            {
                ClientName = "IntegrationTestClient",
                ApiKey = "test-api-key"
            });

            db.SaveChanges();
        }

        private static void SeedUsers(AppDbContext db)
        {
            if (db.Users.Any())
                return;

            db.Users.Add(new User
            {
                UserName = "testuser",
                FullName = "Test User",
                Email = "testuser@test.com",
                Password = "DUMMY_HASH_FOR_TESTS"
            });

            db.SaveChanges();
        }
    }
}
