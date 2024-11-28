/* eslint-disable @typescript-eslint/no-unused-vars */
'use client'

import { Note } from "@/types"
import dynamic from 'next/dynamic'
import { useState } from "react"
import { useRouter } from "next/navigation"

interface RowProps {
  note: Note
}

const ComponentNoteForm = dynamic(() => import('@/app/components/NoteForm'))

const Row: React.FC<RowProps> = ({ note }) => {

  const router = useRouter();

  const [showEditForm, setShowEditForm] = useState<boolean>(false)

  const handleArchiveNote = async() => {
    try {
      const response = await fetch(`http://localhost:8090/archived?userId=${JSON.parse(sessionStorage.getItem("userSession") as string)}&noteId=${note.noteId}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      })
      if (response.ok) {
        alert('Note archived')
      } else {
        const userResponse = confirm('Note already archived, Do you want to remove it?')

        if (userResponse) {
          // Remove archived note
          const response = await fetch(`http://localhost:8090/archived?userId=${JSON.parse(sessionStorage.getItem("userSession") as string)}&noteId=${note.noteId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
        })
        if (response.ok) {
          alert('Note removed from archive')
        }
        }
      }
    } catch (error) {
      console.error('Error archiving note:', error)
    }
  }

  const handleDeleteNote = async() => {
    try {
      const userResponse = confirm('Do you want to remove the selected note?')
      if (userResponse) {
        const response = await fetch(`http://localhost:8090/delete?noteId=${note.noteId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
        })
        if (response.ok) {
          alert('Note deleted')
          router.push('/')
        }
      }
    } catch (error) {
      console.error('Error deleting note:', note)
    }
  }

  return (
  <>
    <div
    className="cursor-pointer border border-gray-300 hover:border-cyan-400 my-2 flex items-center rounded-lg bg-gray-100 p-4 shadow-sm">
      <div className="text-ellipsis overflow-hidden flex-grow">
        <p
        onClick={handleArchiveNote}
        title="Archive note?" 
        className="text-lg font-medium mb-2">{note.title}</p>
        <p className="text-gray-700">{note.content}</p>
      </div>
      <div
      className="flex justify-between px-3 w-[102px] bg-slate-200 rounded-[26px]">
        <svg 
        onClick={handleDeleteNote}
        className="cursor-pointer"
        width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M7 21C6.45 21 5.97933 20.8043 5.588 20.413C5.19667 20.0217 5.00067 19.5507 5 19V6H4V4H9V3H15V4H20V6H19V19C19 19.55 18.8043 20.021 18.413 20.413C18.0217 20.805 17.5507 21.0007 17 21H7ZM17 6H7V19H17V6ZM9 17H11V8H9V17ZM13 17H15V8H13V17Z" 
          fill="#F72793"/>
        </svg>
        <svg 
        className="cursor-pointer"
        onClick={
          () => setShowEditForm(!showEditForm)
        }
        width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M5 19H6.425L16.2 9.225L14.775 7.8L5 17.575V19ZM3 21V16.75L16.2 3.575C16.4 3.39167 16.621 3.25 16.863 3.15C17.105 3.05 17.359 3 17.625 3C17.891 3 18.1493 3.05 18.4 3.15C18.6507 3.25 18.8673 3.4 19.05 3.6L20.425 5C20.625 5.18333 20.771 5.4 20.863 5.65C20.955 5.9 21.0007 6.15 21 6.4C21 6.66667 20.9543 6.921 20.863 7.163C20.7717 7.405 20.6257 7.62567 20.425 7.825L7.25 21H3ZM15.475 8.525L14.775 7.8L16.2 9.225L15.475 8.525Z" 
          fill="#28A745"/>
        </svg>
      </div>
      <ul className="px-2 space-x-2 overflow-hidden">
        {note.tags.$values.map((tag) => (
          <li key={tag} className="inline-flex items-center p-2 text-sm font-medium rounded-full bg-cyan-500 text-white">
            {tag}
          </li>
        ))}
      </ul>
    </div>
    { showEditForm && 
      <ComponentNoteForm note={note}/>
    }
  </>
  )
}

export default Row
