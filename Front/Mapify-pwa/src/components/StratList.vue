<script setup lang="ts">
import { useRouter } from 'vue-router'
import StratCard from '@/components/StratCard.vue'
import { useDraftStore } from '@/stores/draft'
import { useFiltersStore } from '@/stores/filters'
import type { StratDetail, StratSummary } from '@/types'

const props = withDefaults(
  defineProps<{
    strats: (StratSummary | StratDetail)[]
    loading?: boolean
    // 'filters' offers clear-all + propose paths; 'offline' only explains the empty state.
    emptyMode?: 'filters' | 'offline'
  }>(),
  { loading: false, emptyMode: 'filters' },
)

const router = useRouter()
const filters = useFiltersStore()
const draft = useDraftStore()

function clearChoices() {
  filters.clearAll()
}

function proposeStrategy() {
  // Pre-fill the constructor with the currently chosen map (Rule 8.2).
  draft.setMap(filters.mapId)
  router.push('/constructor')
}
</script>

<template>
  <div>
    <div v-if="props.loading" class="strat-grid">
      <div v-for="n in 6" :key="n" class="skeleton"></div>
    </div>

    <div v-else-if="props.strats.length === 0" class="empty-state">
      <template v-if="props.emptyMode === 'filters'">
        <p>No strategy matches your current choices.</p>
        <div class="actions">
          <button type="button" class="btn" @click="clearChoices">Clear all choices</button>
          <button type="button" class="btn btn-primary" @click="proposeStrategy">
            Propose this strategy
          </button>
        </div>
      </template>
      <template v-else>
        <p>No strategies are saved on this device yet.</p>
        <p class="muted">Open a strategy while online and a copy is kept here for offline use.</p>
      </template>
    </div>

    <div v-else class="strat-grid">
      <StratCard v-for="strat in props.strats" :key="strat.id" :strat="strat" />
    </div>
  </div>
</template>
