import { defineStore } from 'pinia'
import { useReferenceStore } from '@/stores/reference'
import { SIDES, type Side } from '@/types'

const STORAGE_KEY = 'mapify.draft'

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

// Defensive validators for restored localStorage data: any field with the
// wrong shape falls back to its empty value instead of breaking the draft.
function asId(value: unknown): number | null {
  return typeof value === 'number' && Number.isInteger(value) && value > 0 ? value : null
}

function asSide(value: unknown): Side | null {
  return SIDES.includes(value as Side) ? (value as Side) : null
}

function asText(value: unknown): string {
  return typeof value === 'string' ? value : ''
}

function asIdList(value: unknown): number[] {
  if (!Array.isArray(value)) return []
  return value.filter((v): v is number => typeof v === 'number' && Number.isInteger(v))
}

function asSketch(value: unknown): Sketch {
  if (typeof value !== 'object' || value === null) return { operators: [], notes: [] }
  const raw = value as Record<string, unknown>
  const operators = Array.isArray(raw.operators)
    ? raw.operators.filter(
        (o): o is SketchOperator =>
          typeof o === 'object' &&
          o !== null &&
          typeof (o as SketchOperator).operatorId === 'number' &&
          typeof (o as SketchOperator).x === 'number' &&
          typeof (o as SketchOperator).y === 'number',
      )
    : []
  const notes = Array.isArray(raw.notes)
    ? raw.notes.filter(
        (n): n is SketchNote =>
          typeof n === 'object' &&
          n !== null &&
          typeof (n as SketchNote).text === 'string' &&
          typeof (n as SketchNote).x === 'number' &&
          typeof (n as SketchNote).y === 'number',
      )
    : []
  return { operators, notes }
}

/**
 * The in-progress strategy draft. It survives navigation between the
 * constructor and the submission form (the sketch is an optional part of
 * the same draft, so both flows share it), and it is persisted to
 * localStorage on every mutation so a page reload loses nothing.
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
    restored: false,
  }),
  actions: {
    persist() {
      try {
        localStorage.setItem(
          STORAGE_KEY,
          JSON.stringify({
            mapId: this.mapId,
            bombsiteId: this.bombsiteId,
            side: this.side,
            sketch: this.sketch,
            name: this.name,
            videoUrl: this.videoUrl,
            description: this.description,
            categoryIds: this.categoryIds,
            operatorIds: this.operatorIds,
          }),
        )
      } catch {
        // localStorage may be unavailable; persistence is best-effort.
      }
    },
    restore() {
      if (this.restored) return
      this.restored = true
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (!raw) return
        const data = JSON.parse(raw) as Record<string, unknown>
        this.mapId = asId(data.mapId)
        this.bombsiteId = asId(data.bombsiteId)
        this.side = asSide(data.side)
        this.sketch = asSketch(data.sketch)
        this.name = asText(data.name)
        this.videoUrl = asText(data.videoUrl)
        this.description = asText(data.description)
        this.categoryIds = asIdList(data.categoryIds)
        this.operatorIds = asIdList(data.operatorIds)
      } catch {
        // Corrupted draft data: ignore it and start fresh.
      }
    },
    /**
     * Drops restored ids that no longer exist in the reference data (and any
     * cross-side leftovers). Call once the reference store has finished
     * loading; safe to call at any other time (no-op until then).
     */
    pruneInvalid() {
      const reference = useReferenceStore()
      if (!reference.loaded) return
      const map = reference.mapById(this.mapId)
      if (this.mapId !== null && !map) {
        this.mapId = null
        this.bombsiteId = null
        this.sketch = { operators: [], notes: [] }
      }
      if (this.bombsiteId !== null && !map?.bombsites.some((b) => b.id === this.bombsiteId)) {
        this.bombsiteId = null
      }
      const sideOfOperator = (id: number) =>
        reference.operators.find((o) => o.id === id)?.side ?? null
      const sideOfCategory = (id: number) =>
        reference.categories.find((c) => c.id === id)?.side ?? null
      const matchesSide = (side: Side | null) =>
        side !== null && (this.side === null || side === this.side)
      this.operatorIds = this.operatorIds.filter((id) => matchesSide(sideOfOperator(id)))
      this.categoryIds = this.categoryIds.filter((id) => matchesSide(sideOfCategory(id)))
      this.sketch.operators = this.sketch.operators.filter((item) =>
        matchesSide(sideOfOperator(item.operatorId)),
      )
      this.persist()
    },
    setMap(mapId: number | null) {
      if (this.mapId !== mapId) {
        this.mapId = mapId
        this.bombsiteId = null
        this.sketch = { operators: [], notes: [] }
      }
      this.persist()
    },
    setBombsite(bombsiteId: number | null) {
      this.bombsiteId = bombsiteId
      this.persist()
    },
    setSide(side: Side | null) {
      if (this.side === side) return
      this.side = side
      if (side) {
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
      }
      this.persist()
    },
    placeOperator(operatorId: number, x: number, y: number) {
      this.sketch.operators.push({ operatorId, x, y })
      this.persist()
    },
    placeNote(text: string, x: number, y: number) {
      this.sketch.notes.push({ text, x, y })
      this.persist()
    },
    removeSketchOperator(index: number) {
      this.sketch.operators.splice(index, 1)
      this.persist()
    },
    removeSketchNote(index: number) {
      this.sketch.notes.splice(index, 1)
      this.persist()
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
      try {
        localStorage.removeItem(STORAGE_KEY)
      } catch {
        // Best-effort cleanup.
      }
    },
  },
})
