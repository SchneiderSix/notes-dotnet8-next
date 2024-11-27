using System.ComponentModel.DataAnnotations;

namespace APINotes.Models
{
    public class Tag
    {
        // Primary key
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Name { get; set; } = null!;

        // Notes associated with tag
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
