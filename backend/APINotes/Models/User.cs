﻿using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace APINotes.Models
{
    public class User
    {
        // Primary key
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Username { get; set; } = null!;

        [RegularExpression(@"^^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[\S]{8,}$",
        ErrorMessage = "{0} must contain at least one special character, one number, one upper case and one lower case.")]
        [StringLength(100, ErrorMessage = "{0} value cannot exceed {1} characters.")]
        public string Password { get; set; } = null!;

        // Notes created by the user
        public ICollection<Note> NotesCreated { get; set; } = new List<Note>();

        // Notes archived
        public ICollection<ArchivedNote> ArchivedNotes { get; set; } = new List<ArchivedNote>();

        // Logical delete
        public bool IsActive { get; set; }


    }
}
