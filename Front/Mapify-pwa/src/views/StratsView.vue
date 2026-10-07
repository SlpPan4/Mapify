<script setup lang="ts">
import { computed, onMounted } from 'vue'
import FilterBar from '@/components/FilterBar.vue'
import OfflineNotice from '@/components/OfflineNotice.vue'
import StratCard from '@/components/StratCard.vue'
import StratList from '@/components/StratList.vue'
import { useFiltersStore } from '@/stores/filters'
import { useOfflineStore } from '@/stores/offline'
import { useReferenceStore } from '@/stores/reference'
import { useStratsStore } from '@/stores/strats'
import type { StratSummary } from '@/types'

// The library view shares the filter store but never restores the remembered
// match context — it always starts from the full library.
const filters = useFiltersStore()
const reference = useReferenceStore()
const strats = useStratsStore()
const offline = useOfflineStore()

const visible = computed(() => strats.filtered(filters))
const showOfflineFallback = computed(() => !offline.isOnline && !strats.loaded)

// Relaxed browsing presents the catalog grouped by map, ordered by map name.
interface MapGroup {
  mapName: string
  strats: StratSummary[]
}
const grouped = computed<MapGroup[]>(() => {
  const groups = new Map<string, StratSummary[]>()
  for (const strat of visible.value) {
    const key = strat.map.name
    const bucket = groups.get(key)
    if (bucket) {
      bucket.push(strat)
    } else {
      groups.set(key, [strat])
    }
  }
  return [...groups.entries()]
    .sort(([a], [b]) => a.localeCompare(b))
    .map(([mapName, groupStrats]) => ({ mapName, strats: groupStrats }))
})

onMounted(() => {
  void reference.load()
  void strats.load()
})
</script>

<template>
  <div class="container">
    <h1 class="page-title">Strategy library</h1>
    <p class="page-subtitle">
      The full catalog, grouped by map — browse at your own pace, no round timer here.
    </p>

    <FilterBar />

    <div v-if="strats.error" class="form-error">
      {{ strats.error }}
      <button type="button" class="btn" style="margin-left: 10px" @click="strats.load(true)">
        Retry
      </button>
    </div>

    <OfflineNotice v-if="showOfflineFallback" />
    <template v-else>
      <!-- Loading and empty states stay with StratList; grouped sections render cards directly. -->
      <StratList v-if="strats.loading || visible.length === 0" :strats="visible" :loading="strats.loading" />
      <div v-else>
        <section v-for="group in grouped" :key="group.mapName" class="library-group">
          <h2 class="library-group-title">
            {{ group.mapName }}
            <span class="library-group-count">{{ group.strats.length }}</span>
          </h2>
          <div class="strat-grid">
            <StratCard v-for="strat in group.strats" :key="strat.id" :strat="strat" />
          </div>
        </section>
      </div>
    </template>
  </div>
</template>
