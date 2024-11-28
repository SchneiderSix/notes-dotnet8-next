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

        public DbSet<ArchivedNote> ArchivedNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filter by logical delete
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.IsActive);

            modelBuilder.Entity<Note>()
                .HasQueryFilter(n => n.IsActive);

            // Handle ArchivedNotes keys
            modelBuilder.Entity<ArchivedNote>()
                .HasOne(an => an.User)
                .WithMany(u => u.ArchivedNotes)
                .HasForeignKey(an => an.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArchivedNote>()
                .HasOne(an => an.Note)
                .WithMany(n => n.ArchivedByUsers)
                .HasForeignKey(an => an.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
