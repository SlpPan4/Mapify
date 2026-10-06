import type {
  ApiResponse,
  Category,
  CategorySubmissionPayload,
  GameMap,
  MapWithBombsites,
  Operator,
  StratDetail,
  StratSubmissionPayload,
  StratSummary,
} from '@/types'

/**
 * All requests use relative /api/... URLs. The Vite dev server proxies them
 * to the backend (local dev: http://localhost:5000, Docker: http://backend:5000).
 */
async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const headers = new Headers(init?.headers)
  headers.set('Accept', 'application/json')
  if (init?.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  let response: Response
  try {
    response = await fetch(path, { ...init, headers })
  } catch {
    throw new Error('Network request failed. Check your connection.')
  }

  let envelope: ApiResponse<T> | null = null
  try {
    envelope = (await response.json()) as ApiResponse<T>
  } catch {
    envelope = null
  }

  if (!response.ok) {
    throw new Error(
      envelope?.error ?? envelope?.message ?? `Request failed with status ${response.status}`,
    )
  }
  if (envelope?.error) {
    throw new Error(envelope.error)
  }
  return (envelope ? envelope.data : null) as T
}

export function getMaps(): Promise<GameMap[]> {
  return apiFetch<GameMap[]>('/api/maps')
}

export function getMap(id: number): Promise<MapWithBombsites> {
  return apiFetch<MapWithBombsites>(`/api/maps/${id}`)
}

export function getCategories(): Promise<Category[]> {
  return apiFetch<Category[]>('/api/categories')
}

export function getOperators(): Promise<Operator[]> {
  return apiFetch<Operator[]>('/api/operators')
}

export function getStratSummaries(): Promise<StratSummary[]> {
  return apiFetch<StratSummary[]>('/api/strats/summary')
}

export function getStrat(id: number): Promise<StratDetail> {
  return apiFetch<StratDetail>(`/api/strats/${id}`)
}

export function submitStrat(payload: StratSubmissionPayload): Promise<{ submissionId: number }> {
  return apiFetch<{ submissionId: number }>('/api/submissions/strats', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function submitCategory(
  payload: CategorySubmissionPayload,
): Promise<{ submissionId: number }> {
  return apiFetch<{ submissionId: number }>('/api/submissions/categories', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}
