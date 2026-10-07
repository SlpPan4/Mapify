import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { registerSW } from 'virtual:pwa-register'

import App from './App.vue'
import router from './router'
import { useDraftStore } from './stores/draft'
import './assets/base.css'

// autoUpdate: the new service worker activates as soon as it is available.
registerSW({ immediate: true })

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

// Restore the persisted draft before any component reads it.
useDraftStore(pinia).restore()

app.mount('#app')
