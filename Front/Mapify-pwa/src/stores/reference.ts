import { defineStore } from 'pinia'
import { getCategories, getMap, getMaps, getOperators } from '@/api/client'
import type { Category, MapWithBombsites, Operator, Side } from '@/types'

export const useReferenceStore = defineStore('reference', {
  state: () => ({
    maps: [] as MapWithBombsites[],
    categories: [] as Category[],
    operators: [] as Operator[],
    loading: false,
    loaded: false,
    error: null as string | null,
  }),
  getters: {
    categoriesForSide:
      (state) =>
      (side: Side | null): Category[] =>
        side ? state.categories.filter((c) => c.side === side) : state.categories,
    operatorsForSide:
      (state) =>
      (side: Side | null): Operator[] =>
        side ? state.operators.filter((o) => o.side === side) : state.operators,
    mapById:
      (state) =>
      (id: number | null): MapWithBombsites | undefined =>
        id === null ? undefined : state.maps.find((m) => m.id === id),
  },
  actions: {
    async load(force = false) {
      if (this.loading || (this.loaded && !force)) return
      this.loading = true
      this.error = null
      try {
        const [maps, categories, operators] = await Promise.all([
          getMaps(),
          getCategories(),
          getOperators(),
        ])
        // GET /api/maps has no bombsites; fetch each map's detail. A map whose
        // detail fetch fails still appears, just without bombsite options.
        this.maps = await Promise.all(
          maps.map((m) => getMap(m.id).catch(() => ({ ...m, bombsites: [] }))),
        )
        this.categories = categories
        this.operators = operators
        this.loaded = true
      } catch (e) {
        this.error = e instanceof Error ? e.message : 'Failed to load reference data'
      } finally {
        this.loading = false
      }
    },
  },
})
