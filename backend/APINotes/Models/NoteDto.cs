namespace APINotes.Models
{
    public class NoteDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<string> Tags { get; set; } = new List<string>();

        public class NoteResponse
        {
            public List<NoteDto> Notes { get; set; } = new List<NoteDto>();
        }
    }
}
