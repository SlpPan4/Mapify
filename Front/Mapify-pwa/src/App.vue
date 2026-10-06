<script setup lang="ts">
import { onMounted } from 'vue'
import { useOfflineStore } from '@/stores/offline'
import { useReferenceStore } from '@/stores/reference'
import { useStratsStore } from '@/stores/strats'

const offline = useOfflineStore()
const reference = useReferenceStore()
const strats = useStratsStore()

onMounted(() => {
  offline.init()
  // Preload the library and reference data so narrowing is instant (Rule 6.2).
  void reference.load()
  void strats.load()
})
</script>

<template>
  <header class="app-header">
    <div class="app-header-inner">
      <router-link to="/" class="brand">Map<span class="brand-accent">ify</span></router-link>
      <nav class="nav">
        <router-link to="/" class="nav-link">Home</router-link>
        <router-link to="/strats" class="nav-link">Library</router-link>
        <router-link to="/constructor" class="nav-link">Constructor</router-link>
        <router-link to="/submit" class="nav-link">Submit</router-link>
        <router-link to="/offline" class="nav-link">Offline</router-link>
      </nav>
    </div>
    <div v-if="!offline.isOnline" class="banner-offline">
      You are offline — only strategies saved on this device are fully available.
    </div>
  </header>

  <main style="flex: 1">
    <RouterView />
  </main>
</template>
