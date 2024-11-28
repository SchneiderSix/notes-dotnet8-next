'use client'

import {HiLogout} from 'react-icons/hi'
import { useRouter } from 'next/navigation';

export default function Header() {

  const router = useRouter();

  const handleLogout = async() => {
    const sessionData = sessionStorage.getItem("userSession")
    sessionStorage.clear()
    router.push('/')
  }

  return (
    <>
      <header className="text-4xl flex justify-center items-center bg-cyan-500 shadow-lg shadow-cyan-500/50 text-white p-10">
        <h3>
          Notes
        </h3>
        <HiLogout title="logout" className="mx-2" size={40} onClick={handleLogout}/>
      </header>
    </>
  )
}