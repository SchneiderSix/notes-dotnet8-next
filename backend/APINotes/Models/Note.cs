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

        // Foreign key for the user who created the note
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Tags associated with note
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        // Note archived by users
        public ICollection<ArchivedNote> ArchivedByUsers { get; set; } = new List<ArchivedNote>();

        // Logical delete
        public bool IsActive { get; set; }
    }
}
