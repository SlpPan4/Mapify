import { defineStore } from 'pinia'
import { useReferenceStore } from '@/stores/reference'
import { SIDES, type Side } from '@/types'

const STORAGE_KEY = 'mapify.context'

type QueryValue = string | (string | null)[] | null | undefined

function toNumber(value: QueryValue): number | null {
  const raw = Array.isArray(value) ? value[0] : value
  if (raw === null || raw === undefined || raw === '') return null
  const n = Number(raw)
  return Number.isInteger(n) && n > 0 ? n : null
}

function toSide(value: QueryValue): Side | null {
  const raw = Array.isArray(value) ? value[0] : value
  return SIDES.includes(raw as Side) ? (raw as Side) : null
}

export const useFiltersStore = defineStore('filters', {
  state: () => ({
    mapId: null as number | null,
    side: null as Side | null,
    bombsiteId: null as number | null,
    categoryId: null as number | null,
    operatorId: null as number | null,
    q: '',
  }),
  getters: {
    hasActive: (state) =>
      state.mapId !== null ||
      state.side !== null ||
      state.bombsiteId !== null ||
      state.categoryId !== null ||
      state.operatorId !== null ||
      state.q.trim() !== '',
  },
  actions: {
    setMap(mapId: number | null) {
      if (this.mapId !== mapId) {
        this.mapId = mapId
        // A bombsite only makes sense within its own map.
        this.bombsiteId = null
      }
      this.persistContext()
    },
    setSide(side: Side | null) {
      this.side = side
      if (side) {
        // Rule 3.2: switching sides discards choices that belong to the other side.
        const reference = useReferenceStore()
        const category = reference.categories.find((c) => c.id === this.categoryId)
        if (category && category.side !== side) this.categoryId = null
        const operator = reference.operators.find((o) => o.id === this.operatorId)
        if (operator && operator.side !== side) this.operatorId = null
      } else {
        this.categoryId = null
        this.operatorId = null
      }
      this.persistContext()
    },
    setBombsite(bombsiteId: number | null) {
      this.bombsiteId = bombsiteId
    },
    setCategory(categoryId: number | null) {
      this.categoryId = categoryId
    },
    setOperator(operatorId: number | null) {
      this.operatorId = operatorId
    },
    setQuery(q: string) {
      this.q = q
    },
    clearAll() {
      this.mapId = null
      this.side = null
      this.bombsiteId = null
      this.categoryId = null
      this.operatorId = null
      this.q = ''
      // Clearing also forgets the remembered context for the next visit (Rule 12.2).
      this.persistContext()
    },
    persistContext() {
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify({ mapId: this.mapId, side: this.side }))
      } catch {
        // localStorage may be unavailable; remembering context is best-effort.
      }
    },
    restoreContext() {
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (!raw) return
        const parsed = JSON.parse(raw) as { mapId?: unknown; side?: unknown }
        const mapId = typeof parsed.mapId === 'number' ? parsed.mapId : null
        const side = SIDES.includes(parsed.side as Side) ? (parsed.side as Side) : null
        this.mapId = mapId
        this.side = side
      } catch {
        // Corrupted or unavailable storage: start with no active choices.
      }
    },
    applyQuery(query: Record<string, QueryValue>) {
      this.mapId = toNumber(query.map)
      this.side = toSide(query.side)
      this.bombsiteId = toNumber(query.site)
      this.categoryId = toNumber(query.category)
      this.operatorId = toNumber(query.operator)
      this.q = (Array.isArray(query.q) ? query.q[0] : query.q) ?? ''
    },
    toQuery(): Record<string, string> {
      const query: Record<string, string> = {}
      if (this.mapId !== null) query.map = String(this.mapId)
      if (this.side !== null) query.side = this.side
      if (this.bombsiteId !== null) query.site = String(this.bombsiteId)
      if (this.categoryId !== null) query.category = String(this.categoryId)
      if (this.operatorId !== null) query.operator = String(this.operatorId)
      if (this.q.trim() !== '') query.q = this.q
      return query
    },
  },
})
