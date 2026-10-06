<script setup lang="ts">
import { computed, onMounted } from 'vue'
import FilterBar from '@/components/FilterBar.vue'
import OfflineNotice from '@/components/OfflineNotice.vue'
import StratList from '@/components/StratList.vue'
import { useFiltersStore } from '@/stores/filters'
import { useOfflineStore } from '@/stores/offline'
import { useReferenceStore } from '@/stores/reference'
import { useStratsStore } from '@/stores/strats'

// The library view shares the filter store but never restores the remembered
// match context — it always starts from the full library.
const filters = useFiltersStore()
const reference = useReferenceStore()
const strats = useStratsStore()
const offline = useOfflineStore()

const visible = computed(() => strats.filtered(filters))
const showOfflineFallback = computed(() => !offline.isOnline && !strats.loaded)

onMounted(() => {
  void reference.load()
  void strats.load()
})
</script>

<template>
  <div class="container">
    <h1 class="page-title">Strategy library</h1>
    <p class="page-subtitle">Browse every published strategy, without match-time pressure.</p>

    <FilterBar />

    <div v-if="strats.error" class="form-error">
      {{ strats.error }}
      <button type="button" class="btn" style="margin-left: 10px" @click="strats.load(true)">
        Retry
      </button>
    </div>

    <OfflineNotice v-if="showOfflineFallback" />
    <StratList v-else :strats="visible" :loading="strats.loading" />
  </div>
</template>
