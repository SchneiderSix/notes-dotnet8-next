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

        // Categories
        public List<Tag> Tags { get; } = new();

        // Handle logic delete
        public bool IsActive { get; set; }

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
