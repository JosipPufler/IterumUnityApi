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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
