using System.ComponentModel.DataAnnotations;

namespace APINotes.Models
{
    public class ArchivedNote
    {
        [Key]
        public Guid Id { get; set; }

        // Foreign keys
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid NoteId { get; set; }
        public Note Note { get; set; } = null!;
    }
}
