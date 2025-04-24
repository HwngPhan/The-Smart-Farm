using Microsoft.EntityFrameworkCore;
using TSF_mustidisProj.Models;

namespace TSF_mustidisProj.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Feed> Feeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            // Configure Feed entity
            modelBuilder.Entity<Feed>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Feed>()
                .Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Feed>()
                .Property(f => f.Key)
                .IsRequired()
                .HasMaxLength(100);

            // Configure relationship
            modelBuilder.Entity<Feed>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feeds)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
