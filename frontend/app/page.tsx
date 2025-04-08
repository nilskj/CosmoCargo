import Link from 'next/link'
import React from 'react'

export default function Home() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-24">
      <div className="z-10 max-w-5xl w-full items-center justify-center font-mono text-sm">
        <h1 className="text-4xl font-bold text-center mb-8">
          🚀 CosmoCargo™ – Intergalaktisk Fraktcentral
        </h1>
        
        <div className="card mb-8">
          <p className="text-xl mb-4">
            Välkommen till CosmoCargo™, den ledande aktören inom rymdlogistik med leveranser till över 9000 rymdstationer.
          </p>
          
          <div className="flex justify-center space-x-4 mt-8">
            <Link href="/login" className="btn-primary">
              Logga in
            </Link>
            <Link href="/register" className="btn-secondary">
              Registrera
            </Link>
          </div>
        </div>
      </div>
    </main>
  )
} 