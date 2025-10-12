import './styles.css'
import 'preline'   // ativa os comportamentos dos componentes (accordions, overlay etc.)

// Exemplo: consumir a API e renderizar em um <div id="app">
// async function loadData() {
//     const res = await fetch('/api/products')
//     const data = await res.json()
//     const el = document.getElementById('app')
//     if (!el) return
//     el.innerHTML = `
//     <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
//       ${data.map((p: any) => `
//         <div class="border rounded-2xl p-4 shadow-sm hover:shadow transition">
//           <h3 class="font-semibold text-lg">${p.name}</h3>
//           <p class="text-sm text-gray-500">${p.description ?? ''}</p>
//           <span class="mt-2 inline-block font-mono">${p.price?.toLocaleString('pt-BR',{style:'currency',currency:'BRL'}) ?? ''}</span>
//         </div>
//       `).join('')}
//     </div>
//   `
// }

(window as any).HSStaticMethods?.autoInit?.();

// loadData()
