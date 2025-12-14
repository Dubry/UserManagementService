using Microsoft.EntityFrameworkCore;
using UserManagementService.Models;

namespace UserManagementService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ApiClient>()
                .Property(u => u.Id);

            modelBuilder.Entity<ApiClient>().HasData(
                new ApiClient
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ClientName = "Swagger",
                    ApiKey = "swagger-dev-key",
                    IsActive = true
                }
            );
        }
    }
}
