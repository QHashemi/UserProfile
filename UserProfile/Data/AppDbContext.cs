using Microsoft.EntityFrameworkCore;
using UserProfile.Entities;

namespace UserProfile.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<UserDetail> UserDetails => Set<UserDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-one: User -> UserDetail
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserDetails)
                .WithOne(d => d.User)
                .HasForeignKey<UserDetail>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: User -> Post
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
