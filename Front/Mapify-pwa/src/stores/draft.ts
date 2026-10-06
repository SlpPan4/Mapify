import { defineStore } from 'pinia'
import { useReferenceStore } from '@/stores/reference'
import type { Side } from '@/types'

export interface SketchOperator {
  operatorId: number
  x: number
  y: number
}

export interface SketchNote {
  text: string
  x: number
  y: number
}

export interface Sketch {
  operators: SketchOperator[]
  notes: SketchNote[]
}

/**
 * The in-progress strategy draft. It survives navigation between the
 * constructor and the submission form (the sketch is an optional part of
 * the same draft, so both flows share it).
 */
export const useDraftStore = defineStore('draft', {
  state: () => ({
    mapId: null as number | null,
    bombsiteId: null as number | null,
    side: null as Side | null,
    sketch: { operators: [], notes: [] } as Sketch,
    name: '',
    videoUrl: '',
    description: '',
    categoryIds: [] as number[],
    operatorIds: [] as number[],
  }),
  actions: {
    setMap(mapId: number | null) {
      if (this.mapId !== mapId) {
        this.mapId = mapId
        this.bombsiteId = null
        this.sketch = { operators: [], notes: [] }
      }
    },
    setBombsite(bombsiteId: number | null) {
      this.bombsiteId = bombsiteId
    },
    setSide(side: Side | null) {
      if (this.side === side) return
      this.side = side
      if (!side) return
      // Switching sides removes everything that belongs to the other side.
      const reference = useReferenceStore()
      const sideOfOperator = (id: number) =>
        reference.operators.find((o) => o.id === id)?.side ?? null
      const sideOfCategory = (id: number) =>
        reference.categories.find((c) => c.id === id)?.side ?? null
      this.sketch.operators = this.sketch.operators.filter(
        (item) => sideOfOperator(item.operatorId) === side,
      )
      this.operatorIds = this.operatorIds.filter((id) => sideOfOperator(id) === side)
      this.categoryIds = this.categoryIds.filter((id) => sideOfCategory(id) === side)
    },
    placeOperator(operatorId: number, x: number, y: number) {
      this.sketch.operators.push({ operatorId, x, y })
    },
    placeNote(text: string, x: number, y: number) {
      this.sketch.notes.push({ text, x, y })
    },
    removeSketchOperator(index: number) {
      this.sketch.operators.splice(index, 1)
    },
    removeSketchNote(index: number) {
      this.sketch.notes.splice(index, 1)
    },
    reset() {
      this.mapId = null
      this.bombsiteId = null
      this.side = null
      this.sketch = { operators: [], notes: [] }
      this.name = ''
      this.videoUrl = ''
      this.description = ''
      this.categoryIds = []
      this.operatorIds = []
    },
  },
})
