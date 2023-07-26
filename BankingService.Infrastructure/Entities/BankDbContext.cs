using Infrastucture.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingService.Infrastructure.Entities
{
    public class BankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("BankInMemory");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasData(new User { UserId = 1, Name = "User1" });
        }
    }
}
