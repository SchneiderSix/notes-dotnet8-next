import { Note } from "@/types"
import { FormEvent } from "react"
import { useRouter } from "next/navigation"

interface NoteFormProps {
  note?: Note
}

const NoteForm: React.FC<NoteFormProps> = ({ note }) => {

  const router = useRouter();

  const handleSubmit = async(e: FormEvent) => {
    e.preventDefault()

    const formData = new FormData(e.target as HTMLFormElement)

    if (
      formData.get('content') == null ||
      formData.get('title') == null ||
      formData.get('tags') == null ||
      (formData.get('title') as string).length > 30 ||
      (formData.get('content') as string).length > 100 ||
      (formData.get('tags') as string).split(' ').length > 5
    ) {
      alert('Title: max 30 char, Content: max 100 char, Tags: max 5')
      return
    }

    if (note) {
      // Edite note
      const noteToEdit = {
        "title": formData.get('title'),
        "content": formData.get('content'),
        "noteId": note.noteId,
        "tags": typeof formData.get('tags') === 'string' ? (formData.get('tags') as string).split(' ') : [],
        "isActive": true
      }

      try {
        const response = await fetch('http://localhost:8090/update', {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(noteToEdit)
        })

        if (response.ok) {
          alert('Note edited')
          router.push('/')
        }
      } catch (error) {
        console.error('Error editing note:', error)
      }
    }else {
      // Create note
      const noteToCreate = {
        "title": formData.get('title'),
        "content": formData.get('content'),
        "userId": JSON.parse(sessionStorage.getItem("userSession") as string),
        "tags": typeof formData.get('tags') === 'string' ? (formData.get('tags') as string).split(' ') : [],
        "isActive": true
      } 
      try {
        const response = await fetch('http://localhost:8090/create', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(noteToCreate)
        })

        if (response.ok) {
          alert('Note created')
          router.push('/')
        }
      } catch (error) {
        console.error('Error editing note:', error)
      }
    }

  }
  

  return (
    <>
      {note ? (
        <form onSubmit={handleSubmit} className="flex flex-col items-start gap-6 w-full">
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Title
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full h-[52px] 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <input 
                name="title"
                type="text" 
                placeholder="Title" 
                defaultValue={note.title}
                className=" font-medium flex-grow border-none outline-none focus:outline-none focus:border-transparent"
              />
            </div>
          </div>
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Content
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <textarea 
                name="content" 
                placeholder="Content" 
                defaultValue={note.content}
                rows={6}
                className=" font-medium flex-grow border-none outline-none 
                focus:outline-none focus:border-transparent 
                w-full h-[70px] resize-none bg-transparent"
              />
            </div>
          </div>
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Tags, separated by space
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full h-[52px] 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <input 
                name="tags"
                type="text" 
                placeholder="Tags" 
                defaultValue={note.tags.$values.join(' ')}
                className=" font-medium flex-grow border-none outline-none focus:outline-none focus:border-transparent"
              />
            </div>
          </div>
          <div className="flex flex-col items-end w-full gap-3">
            <button type="submit" className="flex justify-center items-center 
              px-[49px] py-[14px] gap-2 
              w-full h-[52px] 
              bg-cyan-500 text-white rounded-[6px]
              hover:bg-cyan-700 transition-colors duration-200">
              Edit
            </button>
          </div>
        </form>
      ) : (
        <form onSubmit={handleSubmit} className="flex flex-col items-start gap-6 w-full">
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Title
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full h-[52px] 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <input 
                name="title"
                type="text" 
                placeholder="Title" 
                className=" font-medium flex-grow border-none outline-none focus:outline-none focus:border-transparent"
              />
            </div>
          </div>
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Content
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <textarea 
                name="content" 
                placeholder="Content"
                rows={6}
                className=" font-medium flex-grow border-none outline-none 
                focus:outline-none focus:border-transparent 
                w-full h-[70px] resize-none bg-transparent"
              />
            </div>
          </div>
          <div className="w-full space-y-2">
            <label className=" font-medium  text-[#282828]">
              Tags, separated by space
            </label>
            <div className="box-border flex items-center p-4 gap-2 
                w-full h-[52px] 
                bg-white border border-[#E8E9EA] rounded-[6px] 
                focus-within:border-cyan-500">
              <input 
                name="tags"
                type="text" 
                placeholder="Tag" 
                className=" font-medium flex-grow border-none outline-none focus:outline-none focus:border-transparent"
              />
            </div>
          </div>
          <div className="flex flex-col items-end w-full gap-3">
            <button type="submit" className="flex justify-center items-center 
              px-[49px] py-[14px] gap-2 
              w-full h-[52px] 
              bg-cyan-500 text-white rounded-[6px]
              hover:bg-cyan-700 transition-colors duration-200">
              Create
            </button>
          </div>
        </form>
      )}
    </>
  )
}

export default NoteForm