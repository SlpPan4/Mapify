export type Side = 'Attack' | 'Defense'

export const SIDES: Side[] = ['Attack', 'Defense']

export interface GameMap {
  id: number
  name: string
}

export interface Bombsite {
  id: number
  mapId: number
  name: string
}

export interface MapWithBombsites extends GameMap {
  bombsites: Bombsite[]
}

export interface Category {
  id: number
  name: string
  side: Side
}

export interface Operator {
  id: number
  name: string
  side: Side
}

export interface StratSummary {
  id: number
  name: string
  videoUrl: string
  description: string
  map: GameMap
  bombsite: Bombsite | null
  side: Side
  categories: Category[]
  operators: Operator[]
}

export interface StratDetail {
  id: number
  name: string
  videoUrl: string
  description: string
  map: GameMap
  bombsite: Bombsite | null
  categories: Category[]
  operators: Operator[]
}

export interface ApiResponse<T> {
  status: number
  data: T | null
  message: string | null
  error: string | null
}

export interface StratSubmissionPayload {
  name: string
  videoUrl: string
  mapId: number
  bombsiteId: number | null
  description: string
  categoryIds: number[]
  operatorIds: number[]
}

export interface CategorySubmissionPayload {
  name: string
  side: Side
}

/** Derives the side from related entities; the detail endpoint has no explicit side field. */
export function deriveSide(strat: { categories: Category[]; operators: Operator[] }): Side | null {
  return strat.categories[0]?.side ?? strat.operators[0]?.side ?? null
}
