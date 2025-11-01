using IterumApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IterumApi
{
    public class IterumDbContext : DbContext
    {
        public IterumDbContext(DbContextOptions<IterumDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();
            modelBuilder.Entity<Role>()
                .HasIndex(role => role.Name)
                .IsUnique();
        }
    }
}
