<script setup lang="ts">
import { computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FilterBar from '@/components/FilterBar.vue'
import MapGrid from '@/components/MapGrid.vue'
import OfflineNotice from '@/components/OfflineNotice.vue'
import QrShare from '@/components/QrShare.vue'
import StratList from '@/components/StratList.vue'
import { useFiltersStore } from '@/stores/filters'
import { useOfflineStore } from '@/stores/offline'
import { useReferenceStore } from '@/stores/reference'
import { useStratsStore } from '@/stores/strats'

const route = useRoute()
const router = useRouter()
const filters = useFiltersStore()
const reference = useReferenceStore()
const strats = useStratsStore()
const offline = useOfflineStore()

const QUERY_KEYS = ['map', 'side', 'site', 'category', 'operator', 'q']

const visible = computed(() => strats.filtered(filters))
const showOfflineFallback = computed(() => !offline.isOnline && !strats.loaded)

onMounted(async () => {
  // A shared link wins over the remembered context (Rule 12.1).
  const hasQuery = QUERY_KEYS.some((key) => route.query[key] !== undefined)
  if (hasQuery) {
    filters.applyQuery(route.query)
  } else {
    filters.restoreContext()
  }
  await Promise.all([reference.load(), strats.load()])
  // Reference data is needed to drop cross-side choices from handcrafted URLs.
  filters.setSide(filters.side)
})

// Mirror the active choices into query params so a narrowed view is shareable.
watch(
  () => ({ ...filters.$state }),
  () => {
    const query = filters.toQuery()
    if (JSON.stringify(query) !== JSON.stringify(route.query)) {
      void router.replace({ query })
    }
  },
)
</script>

<template>
  <div class="container">
    <h1 class="page-title">Find your strategy</h1>
    <p class="page-subtitle">Pick a map, narrow down, play — in under 30 seconds.</p>

    <h2 class="step-heading"><span class="step-number">1</span> Pick the map</h2>
    <MapGrid />

    <h2 class="step-heading">
      <span class="step-number">2</span> Pick your side
      <span v-if="filters.side" class="step-done">· {{ filters.side }}</span>
    </h2>
    <FilterBar variant="match" />

    <p v-if="filters.mapId !== null" class="step-hint">
      Narrow down further above — bombsite, category, operator or name — or open a strategy below.
    </p>

    <div class="detail-actions">
      <QrShare label="Share this view" />
    </div>

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
