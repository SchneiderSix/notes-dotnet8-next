namespace APINotes.Models
{
    public class ArchivedNoteDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<string> Tags { get; set; } = new List<string>();
    }
    public class ArchivedNoteResponse
    {
        public List<ArchivedNoteDto> Notes { get; set; } = new List<ArchivedNoteDto>();
    }
}
