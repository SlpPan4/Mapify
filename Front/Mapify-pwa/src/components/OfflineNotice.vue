<script setup lang="ts">
defineProps<{
  message?: string
}>()
</script>

<template>
  <div class="offline-notice">
    <p>{{ message ?? 'This content is unavailable without an internet connection.' }}</p>
    <p>
      <router-link to="/offline">View strategies saved on this device</router-link>
    </p>
  </div>
</template>
