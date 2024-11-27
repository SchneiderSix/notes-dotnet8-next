using APINotes.Data;
using APINotes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            var tags = await _context.Tags.Include(t => t.Notes).ToListAsync();

            if (tags == null) return BadRequest("No tags found");

            return Ok(tags);
        }

        [HttpGet("/user")]
        public async Task<ActionResult<List<Note>>> GetNotesFromUser([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest("Invalid credentials");

            var notes = await _context.Users
                .Where(u => u.Username == username && u.IsActive)
                .SelectMany(u => u.NotesCreated)
                .ToListAsync();

            if (notes == null) return BadRequest("No notes found for the user or user does not exist");

            return Ok(notes);
        }

        //[HttpGet("/archived")]
        //[HttpPut("/archived")]
        //[HttpPut]

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
    }
}
