import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// La API vive en otro proceso (ASP.NET Core). El origen se configura por
// variable de entorno VITE_API_URL; ver .env.example.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
  },
})
