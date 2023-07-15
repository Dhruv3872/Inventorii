using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inventorii.Models;

namespace Inventorii.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
<<<<<<< HEAD
<<<<<<< Updated upstream
=======

        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.firstName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.lastName)
                .HasMaxLength(250);

        }
>>>>>>> Stashed changes
=======

        public DbSet<Item> Items { get; set; } = null!;
>>>>>>> 1f6b739a5a80686a831b40a0b34d225be83fc681
    }
}