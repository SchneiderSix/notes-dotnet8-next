export interface NoteAPIResponse {
  notes: {
    $values: Array<{
      noteId: string,
      userId: string,
      title: string,
      content: string,
      tags: {
        $values: string[]
      }
    }>
  }
}