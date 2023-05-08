import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  isProduction: false,
  // https://github.com/vitejs/vite/issues/4116
  server: {
    host: true,
    port: 5000,
    strictPort: true,
  },
  preview: {
    port: 5000,
    strictPort: true,
    open : true,
  },
})
