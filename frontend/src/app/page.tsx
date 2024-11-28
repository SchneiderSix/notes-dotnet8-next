/* eslint-disable */
"use client"
import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function Home() {

  const router = useRouter();

  useEffect(() => {
    if (typeof window !== "undefined") {
      const sessionData = sessionStorage.getItem("userSession");
      if (sessionData) {
        router.push('/home');
      } else {
        router.push('/login');
      }
    }
  }, []);

  return (
    <>
    </>
  );
}