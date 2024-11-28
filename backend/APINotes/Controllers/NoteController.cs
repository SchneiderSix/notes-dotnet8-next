using APINotes.Data;
using APINotes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static APINotes.Models.NoteDto;

namespace APINotes.Controllers
{
    public class CreateNoteModel
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string[] Tags { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateNoteModel
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string NoteId { get; set; } = null!;
        public string[] Tags { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    [Route("api/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly DataContext _context;

        public NoteController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tag>>> GetNotes()
        {
            var notes = await _context.Users
                .Where(u => u.IsActive)
                .SelectMany(u => u.NotesCreated)
                .Include(n => n.Tags)
                .ToListAsync();

            if (notes.Count == 0) return BadRequest("No notes found");

            var result = notes.Select(note => new NoteDto
            {
                NoteId = note.Id,
                Title = note.Title,
                Content = note.Content,
                Tags = note.Tags.Select(t => t.Name).ToList() // Correctly mapping Tags
            }).ToList();

            return Ok(new NoteResponse { Notes = result });
        }

        [HttpGet("/user")]
        public async Task<ActionResult<List<Note>>> GetNotesFromUser([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest("Invalid credentials");

            var notes = await _context.Users
                .Where(u => u.Username == username && u.IsActive)
                .SelectMany(u => u.NotesCreated)
                .Include(n => n.Tags)
                .ToListAsync();

            if (notes.Count == 0) return BadRequest("No notes found for the user or user does not exist");

            // Project from custom DTO
            var result = notes.Select(note => new NoteDto
            {
                NoteId = note.Id,
                Title = note.Title,
                Content = note.Content,
                Tags = note.Tags.Select(t => t.Name).ToList()
            }).ToList();

            return Ok(new NoteResponse { Notes = result });
        }

        [HttpGet("/archived")]
        public async Task<ActionResult<List<ArchivedNote>>> GetArchivedNotesFromUser([FromQuery] Guid  userId)
        {
            var notes = await _context.Users
                .Where(u => u.Id == userId && u.IsActive)
                .SelectMany(u => u.ArchivedNotes)
                .Include(n => n.Note)
                .ThenInclude(n => n.Tags)
                .ToListAsync();

            if (notes == null || !notes.Any()) return BadRequest("No notes found");

            // Project from custom DTO
            var result = notes.Select(an => new ArchivedNoteDto
            {
                NoteId = an.Note.Id,
                Title = an.Note.Title,
                Content = an.Note.Content,
                Tags = an.Note.Tags.Select(t => t.Name).ToList()
            }).ToList();

            return Ok(new ArchivedNoteResponse { Notes = result });
        }

        [HttpPost("/archived")]
        public async Task<ActionResult<string>> ArchiveNote([FromQuery] Guid userId, Guid noteId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return BadRequest("User not found");

            var note = await _context.Notes.FindAsync(noteId);
            if (note == null) return BadRequest("Note not found");


            // Check if already archived
            var alreadyArchived = await _context.ArchivedNotes
                .AnyAsync(an => an.UserId == userId && an.NoteId == noteId);

            if (alreadyArchived) return BadRequest("Note already archived by this user");

            var archivedNote = new ArchivedNote
            {
                UserId = userId,
                NoteId = noteId,
            };

            _context.ArchivedNotes.Add(archivedNote);
            await _context.SaveChangesAsync();

            return Ok("Note archived");
        }

        [HttpPut("/archived")]
        public async Task<ActionResult<string>> DeleteArchivedNoteFromUser([FromQuery] Guid userId, Guid noteId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return BadRequest("User not found");

            var note = await _context.Notes.FindAsync(noteId);
            if (note == null) return BadRequest("Note not found");

            // Check if already archived
            var archivedNote = await _context.ArchivedNotes
                .FirstOrDefaultAsync(an => an.UserId == userId && an.NoteId == noteId);

            if (archivedNote == null) return BadRequest("This note isn't archived by this user");

            // delete archived note
            _context.ArchivedNotes.Remove(archivedNote);
            await _context.SaveChangesAsync();

            return Ok("Note removed from archive");
        }

        [HttpPost("/create")]
        public async Task<ActionResult<Note>> CreateNote([FromBody] CreateNoteModel noteModel)
        {
            if (
                string.IsNullOrEmpty(noteModel.Title) ||
                string.IsNullOrEmpty(noteModel.Content) ||
                string.IsNullOrEmpty(noteModel.UserId) ||
                noteModel.Tags == null || noteModel.Tags.Length == 0
            )
            {
                return BadRequest("Invalid credentials");
            }

            // Convert userId (which is a string) to GUID
            if (!Guid.TryParse(noteModel.UserId, out var userGuid))
            {
                return BadRequest("Invalid UserId");
            }

            // Ensure the user exists
            var user = await _context.Users.FindAsync(userGuid);
            if (user == null)
                return BadRequest("User not found");

            // Create the note object
            var note = new Note
            {
                Title = noteModel.Title,
                Content = noteModel.Content,
                UserId = userGuid, // Ensuring UserId is set
                User = user, // Associating the user with the note
                IsActive = noteModel.IsActive,
                Tags = new List<Tag>()
            };

            var uniqueTags = new HashSet<string>(noteModel.Tags);

            // Handle tags
            foreach (var tagName in uniqueTags)
            {
                // Check if the tag exists
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (existingTag == null)
                {
                    // If tag doesn't exist, create a new tag
                    var newTag = new Tag { Name = tagName, Notes = new List<Note>() };
                    newTag.Notes.Add(note); // Add note to the tag
                    _context.Tags.Add(newTag); // Add the tag to the database
                    note.Tags.Add(newTag); // Add the new tag to the note
                }
                else
                {
                    // If tag exists, add it to the note
                    note.Tags.Add(existingTag);
                }
            }

            // Add the note to the context (note is now associated with its tags and user)
            _context.Notes.Add(note);

            // Save changes (note + tags + user)
            await _context.SaveChangesAsync();

            return Ok("Note created");
        }

        [HttpPut("/update")]
        public async Task<ActionResult<string>> UpdateNote([FromBody] UpdateNoteModel noteModel)
        {
            if (
                string.IsNullOrEmpty(noteModel.NoteId) ||
                string.IsNullOrEmpty(noteModel.Title) ||
                string.IsNullOrEmpty(noteModel.Content) ||
                noteModel.Tags == null || noteModel.Tags.Length == 0
            )
            {
                return BadRequest("Invalid input");
            }

            // Convert noteId (string) to GUID
            if (!Guid.TryParse(noteModel.NoteId, out var noteGuid))
            {
                return BadRequest("Invalid NoteId");
            }

            // Retrieve the note to be updated
            var note = await _context.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == noteGuid);
            if (note == null)
                return NotFound("Note not found");

            // Update note fields
            note.Title = noteModel.Title;
            note.Content = noteModel.Content;
            note.IsActive = noteModel.IsActive;

            // Handle tags logic
            var uniqueTags = new HashSet<string>(noteModel.Tags);
            var currentTags = note.Tags.ToList();

            // Remove tags that are no longer associated with the note
            foreach (var tag in currentTags)
            {
                if (!uniqueTags.Contains(tag.Name))
                {
                    note.Tags.Remove(tag);

                    // Optionally: Remove the tag from the database if it has no other associated notes
                    if (!await _context.Notes.AnyAsync(n => n.Tags.Any(t => t.Id == tag.Id)))
                    {
                        _context.Tags.Remove(tag);
                    }
                }
            }

            // Add new tags that are not already associated with the note
            foreach (var tagName in uniqueTags)
            {
                if (!note.Tags.Any(t => t.Name == tagName))
                {
                    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (existingTag == null)
                    {
                        // Create a new tag if it doesn't exist
                        var newTag = new Tag { Name = tagName, Notes = new List<Note>() };
                        newTag.Notes.Add(note);
                        _context.Tags.Add(newTag);
                        note.Tags.Add(newTag);
                    }
                    else
                    {
                        // Associate the existing tag with the note
                        note.Tags.Add(existingTag);
                    }
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("Note updated successfully");
        }
    }
}
