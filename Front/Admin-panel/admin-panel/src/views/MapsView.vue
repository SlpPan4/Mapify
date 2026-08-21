<script setup>
import PlaceholderPage from '../components/PlaceholderPage.vue'

const icon = `<svg class="w-7 h-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5"><path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" /><path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" /></svg>`
</script>

<template>
  <PlaceholderPage
    title="Maps Management"
    description="View and manage all available maps in the Mapify database. This section is coming soon."
    :icon="icon"
  />
</template>
