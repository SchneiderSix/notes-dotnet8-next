'use client'

import { Note, NoteAPIResponse } from '@/types'
import dynamic from 'next/dynamic'
import { useState, useEffect } from 'react'
import { useDebounce } from 'use-debounce'

const ComponentSearchBar = dynamic(() => import('@/app/components/SearchBar'))
const ComponentRow = dynamic(() => import('@/app/components/Row'))
const ComponentNoteForm = dynamic(() => import('@/app/components/NoteForm'))

export default function Table() {

  const [notes, setNotes] = useState<Note[] | null>(null)
  const [searchValue, setSearchValue] = useState<string>('')
  const [debouncedSearchValue] = useDebounce(searchValue, 1000)
  const [createNoteForm, setCreateNoteForm] = useState<boolean>(false)

  const filterNotesByTag = (searchValue: string) => {
    if (!notes || !searchValue) return notes;
  
    return notes.filter(note => 
      note.tags.$values.some(tag => tag.toLowerCase().includes(searchValue.toLowerCase()))
    );
  };

  const fetchNotes = async() => {
    try {
      const response = await fetch('http://localhost:8090/api/note', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      })
      if (response.ok) {
        const data: NoteAPIResponse = await response.json()
        setNotes(data.notes.$values)
      } else console.error('Error fetching notes')
    } catch (error) {
      console.error('Error fetching notes:', error)
    }
  }

  const getMyNotes = async() => {
    try {
      const response = await fetch(`http://localhost:8090/user?userId=${JSON.parse(sessionStorage.getItem("userSession") as string)}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      })
      if (response.ok) {
        const data: NoteAPIResponse = await response.json()
        setNotes(data.notes.$values)
      } else console.error('Error fetching notes')
    } catch (error) {
      console.error('Error fetching notes:', error)
    }
  }

  const getArchivedNotes = async() => {
    try {
      const response = await fetch(`http://localhost:8090/archived?userId=${JSON.parse(sessionStorage.getItem("userSession") as string)}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      })
      if (response.ok) {
        const data: NoteAPIResponse = await response.json()
        setNotes(data.notes.$values)
      } else setNotes(null)
    } catch (error) {
      console.error('Error fetching notes:', error)
    }
  }

  useEffect(() => {

    fetchNotes()

  }, [])

  const handleSearch = (value: string) => {
    setSearchValue(value)
  }

  const filteredNotes = filterNotesByTag(debouncedSearchValue);

  return (
    <>
      <div className='flex justify-between p-2 text-lg font-medium'>
        <p
        onClick={
          () => setCreateNoteForm(!createNoteForm)
        } 
        className='cursor-pointer hover:bg-cyan-500 transition-colors duration-200 hover:text-white rounded-lg p-1'>
          Create Note
        </p>
        <p
        onClick={getMyNotes}
        className='cursor-pointer hover:bg-cyan-500 transition-colors duration-200 hover:text-white rounded-lg p-1'>
          My notes
        </p>
        <p
        onClick={getArchivedNotes}
        className='cursor-pointer hover:bg-cyan-500 transition-colors duration-200 hover:text-white rounded-lg p-1'>
          Archived
        </p>
        <ComponentSearchBar onSearch={handleSearch}/>
      </div>
      <div>
        { filteredNotes?.map((i) => (
          <ComponentRow note={i} key={i.noteId} />
        )) }
      </div>
      { createNoteForm && <ComponentNoteForm /> }
    </>
  )
}