using System.ComponentModel.DataAnnotations;

namespace APINotes.Models
{
    public class User
    {
        // Primary key
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Username { get; set; } = null!;

        [RegularExpression(@"^^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[\S]{8,}$",
        ErrorMessage = "{0} must contain at least one special character, one number, one upper case and one lower case.")]
        [StringLength(100, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Password { get; set; } = null!;

        // Notes created
        public List<Note> NotesCreated { get; set; } = new();

        // Notes archived
        public List<Note> NotesArchived { get; set; } = new();

        // Logic delete
        public bool IsActive { get; set; }


    }
}
