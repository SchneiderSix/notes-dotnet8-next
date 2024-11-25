using APINotes.Models;
using Microsoft.EntityFrameworkCore;
namespace APINotes.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                // Set primary key
                entity.HasKey(u => u.Id);

                // Ensure Username is unique
                entity.HasIndex(u => u.Username).IsUnique();

                // Configure relationships
                entity.HasMany(u => u.NotesCreated)
                      .WithOne(n => n.User)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Add global query filter for logical deletion
                entity.HasQueryFilter(u => u.IsActive);
            });

            // Configure Note entity
            modelBuilder.Entity<Note>(entity =>
            {
                // Set primary key
                entity.HasKey(n => n.Id);

                // Configure relationships
                entity.HasMany(n => n.Tags)
                      .WithMany(t => t.Notes);

                // Add global query filter for logical deletion
                entity.HasQueryFilter(n => n.IsActive);

                // Map the foreign key relationship with User
                entity.HasOne(n => n.User)
                      .WithMany(u => u.NotesCreated)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Tag entity
            modelBuilder.Entity<Tag>(entity =>
            {
                // Set primary key
                entity.HasKey(t => t.Id);

                // Configure relationships
                entity.HasMany(t => t.Notes)
                      .WithMany(n => n.Tags);
            });

            // Additional configurations
            base.OnModelCreating(modelBuilder);
        }

    }
}
