using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APINotes.Models
{
    public class Note
    {
        [Key]
        // Primary key
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(250, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Content { get; set; } = null!;

        // Foreign key to the user who created the note
        public Guid UserId { get; set; }

        // Navigation property for the user who created the note
        public virtual User User { get; set; } = null!;

        // Tags associated with this note
        public List<Tag> Tags { get; set; } = new();

        // Archived by user
        public Guid? ArchivedByUserId { get; set; }

        public virtual User? ArchivedByUser { get; set; }

        // Logical delete
        public bool IsActive { get; set; }
    }
}
