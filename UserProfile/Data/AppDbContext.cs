using Microsoft.EntityFrameworkCore;
using UserProfile.Entities;

namespace UserProfile.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // Here we define the Users DbSet to represent the Users table in the database
        // User Entity table
        // DbSets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserDetails> UserDetails { get; set; } = null!;

        // Optional: configure one-to-one relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                // Configure one-to-one relationship between User and UserDetails
                .HasOne(u => u.UserDetails)
                .WithOne(d => d.User)

                // Specify UserDetails.UserId as the foreign key
                .HasForeignKey<UserDetails>(d => d.UserId)

                // Automatically delete UserDetails when the related User is deleted
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}
