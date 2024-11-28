'use client'
import dynamic from 'next/dynamic'
import { useRouter } from 'next/navigation'
import { useEffect } from 'react'

const ComponentHeader = dynamic(() => import('@/app/components/Header'))
const ComponentTable = dynamic(() => import('@/app/components/Table'))

export default function HomePage() {

  const router = useRouter();

  useEffect(() => {
    if (typeof window !== "undefined") {
      const sessionData = sessionStorage.getItem("userSession");
      if (!sessionData) {
        router.push('/');
      }
    }
  }, []);

  return (
    <>
      <ComponentHeader />
      <div className='bg-white shadow-md rounded-lg w-2/3 p-4 mx-auto my-4'>
        <ComponentTable />
      </div>
    </>
  )
}