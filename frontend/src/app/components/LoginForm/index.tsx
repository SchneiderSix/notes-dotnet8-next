import { FormEvent } from "react"
import { useRouter } from "next/navigation"

export default function LoginForm() {

  const router = useRouter();

  const handleSubmit = async(e: FormEvent) => {
    e.preventDefault()

    const formData = new FormData(e.target as HTMLFormElement)

    const user = {
      "username": formData.get('username'),
      "password": formData.get('password'),
      "isActive": true
    }

    if (
      user.username == null ||
      user.password == null
    ) {
      alert('Invalid credentials')
      return
    } else if (user.username.toString.length > 30) {
      alert('Username too long')
      return
    } else if (user.password.toString.length > 100) {
      alert('Password too long')
      return
    } else if(typeof(user.password) === 'string' && !/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[\S]{8,}$/.test(user.password)) {
      alert('Invalid format for password')
      return
    }

    try {
      const response = await fetch('http://localhost:8090/api/user/login', {
        method: "POST",
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)
      })

      if (response.status != 200) {
        const contentType = response.headers.get("content-type");
        const errorMessage = contentType && contentType.includes("application/json")
          ? (await response.json()).message
          : await response.text()
    
        alert(errorMessage)
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data = await response.json()
      sessionStorage.setItem('userSession', JSON.stringify(data))

      const storedSession = JSON.parse(sessionStorage.getItem('userSession') || '{}')

      console.log(storedSession)

      router.push('/home')

    } catch (error) {
      console.log('Error fetching user:', error)
    }
  }

  return (
    <div className="flex justify-center items-center h-screen">
    <div className="bg-white shadow-md rounded-lg w-1/4">
      <form onSubmit={handleSubmit}>
        <div className="mb-4 p-5">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="email">
            Username
          </label>
          <input
            className='mt-1 block w-full px-3 py-2 bg-white border border-slate-300 rounded-md text-sm shadow-sm placeholder-slate-400
            focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500
            disabled:bg-slate-50 disabled:text-slate-500 disabled:border-slate-200 disabled:shadow-none
            invalid:border-pink-500 invalid:text-pink-600
            focus:invalid:border-pink-500 focus:invalid:ring-pink-500'
            name="username"
            type="text"
            placeholder="Enter your username"
          />
        </div>
        <div className="mb-4 p-5">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="password">
            Password
          </label>
          <input
            className='mt-1 block w-full px-3 py-2 bg-white border border-slate-300 rounded-md text-sm shadow-sm placeholder-slate-400
            focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500
            disabled:bg-slate-50 disabled:text-slate-500 disabled:border-slate-200 disabled:shadow-none
            invalid:border-pink-500 invalid:text-pink-600
            focus:invalid:border-pink-500 focus:invalid:ring-pink-500'
            name="password"
            type="password"
            placeholder="Enter your password"
          />
        </div>
        <div className="px-4 pb-4">
        <button
            className="bg-cyan-500 shadow-lg shadow-cyan-500/50 hover:bg-[#0081A7] text-white font-bold py-2 px-4 rounded-lg focus:outline-none focus:shadow-outline w-full"
            type="submit"
          >
            Login
          </button>
        </div>
        <div className="flex items-center justify-between">
        </div>
      </form>
    </div>
  </div>
  )
}