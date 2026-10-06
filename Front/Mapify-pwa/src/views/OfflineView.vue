<script setup lang="ts">
import { onMounted, ref } from 'vue'
import StratList from '@/components/StratList.vue'
import { useOfflineStore } from '@/stores/offline'
import type { StratDetail } from '@/types'

const offline = useOfflineStore()

const saved = ref<StratDetail[]>([])
const loading = ref(true)

onMounted(async () => {
  saved.value = await offline.listSaved()
  loading.value = false
})
</script>

<template>
  <div class="container">
    <h1 class="page-title">Saved on this device</h1>
    <p class="page-subtitle">
      Every strategy you fully open while online is kept here, so it stays available without a
      connection.
    </p>

    <StratList :strats="saved" :loading="loading" empty-mode="offline" />
  </div>
</template>
