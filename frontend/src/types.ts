export interface Note {
  noteId: string,
      userId: string,
      title: string,
      content: string,
      tags: {
        $values: string[]
      }
}

export interface NoteAPIResponse {
  notes: {
    $values: Note[]
  }
}