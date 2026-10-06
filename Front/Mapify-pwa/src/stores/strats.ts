import { defineStore } from 'pinia'
import { getStratSummaries } from '@/api/client'
import type { Side, StratSummary } from '@/types'

export interface StratFilters {
  mapId: number | null
  bombsiteId: number | null
  side: Side | null
  categoryId: number | null
  operatorId: number | null
  q: string
}

export const useStratsStore = defineStore('strats', {
  state: () => ({
    summaries: [] as StratSummary[],
    loading: false,
    loaded: false,
    error: null as string | null,
  }),
  getters: {
    // All narrowing happens in memory on the summary list, so every filter
    // change is instant and never waits for a network round-trip.
    filtered: (state) => {
      return (f: StratFilters): StratSummary[] => {
        const query = f.q.trim().toLowerCase()
        return state.summaries.filter((s) => {
          if (f.mapId !== null && s.map.id !== f.mapId) return false
          if (f.bombsiteId !== null && s.bombsite?.id !== f.bombsiteId) return false
          if (f.side !== null && s.side !== f.side) return false
          if (f.categoryId !== null && !s.categories.some((c) => c.id === f.categoryId))
            return false
          if (f.operatorId !== null && !s.operators.some((o) => o.id === f.operatorId))
            return false
          if (query && !s.name.toLowerCase().includes(query)) return false
          return true
        })
      }
    },
  },
  actions: {
    async load(force = false) {
      if (this.loading || (this.loaded && !force)) return
      this.loading = true
      this.error = null
      try {
        this.summaries = await getStratSummaries()
        this.loaded = true
      } catch (e) {
        this.error = e instanceof Error ? e.message : 'Failed to load strategies'
      } finally {
        this.loading = false
      }
    },
  },
})
