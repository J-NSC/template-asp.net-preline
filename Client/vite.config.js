import { defineConfig } from 'vite'
import tailwind from '@tailwindcss/vite'

export default defineConfig({
    plugins: [tailwind()],
    server: {
        port: 5173,
        strictPort: true
    },
    build: {
        outDir: '../wwwroot',
        emptyOutDir: false,
        manifest: true,
        assetsDir: 'assets'
    }
})
