'use client'

import dynamic from 'next/dynamic'

const ComponentLoginForm = dynamic(() => import('@/app/components/LoginForm'))

export default function LoginPage() {
  return (
    <>
      <ComponentLoginForm />
    </>
  )
}