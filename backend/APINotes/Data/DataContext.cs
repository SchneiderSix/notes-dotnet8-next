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
            // Global Query Filters for IsActive
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.IsActive);

            modelBuilder.Entity<Note>()
                .HasQueryFilter(n =>  n.IsActive);

            // User -> NotesCreated relationship
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.NotesCreated)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> NotesArchived relationship
            modelBuilder.Entity<Note>()
                .HasOne(n => n.ArchivedByUser)
                .WithMany(u => u.NotesArchived)
                .HasForeignKey(n => n.ArchivedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many: Note <-> Tag
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags)
                .WithMany(t => t.Notes)
                .UsingEntity<Dictionary<string, object>>(
                    "NoteTag",
                    j => j.HasOne<Tag>()
                          .WithMany()
                          .HasForeignKey("TagId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Note>()
                          .WithMany()
                          .HasForeignKey("NoteId")
                          .OnDelete(DeleteBehavior.Cascade)
                )
                .ToTable("NoteTags");

            // Additional configurations
            base.OnModelCreating(modelBuilder);
        }

    }
}
