import { test as base, expect } from '@playwright/test'
import type { Page } from '@playwright/test'

/**
 * Seed data — mirrors the example mappings of the functional design:
 * maps, bombsites, categories, operators and the named strategies
 * ("Cool Ash Rush", "Thermite breach", "Lesion 2f setup").
 * The backend is not part of this repository, so every test mocks the
 * API deterministically via route interception. Each test gets a fresh
 * browser context (clean localStorage + IndexedDB) and a fresh capture
 * buffer, so tests are independent and domain state never leaks.
 */

export const MAPS = [
  { id: 1, name: 'Oregon' },
  { id: 2, name: 'Consulate' },
  { id: 3, name: 'Clubhouse' },
  { id: 4, name: 'Border' },
  { id: 5, name: 'Coastline' },
]

export const BOMBSITES = [
  { id: 101, mapId: 1, name: '2F Main Dorms Hall / 2F Kids Dormitory' },
  { id: 102, mapId: 1, name: '1F Dining Hall / 1F Kitchen' },
  { id: 103, mapId: 1, name: '1F Meeting Hall / 1F Kitchen' },
  { id: 104, mapId: 1, name: 'B Laundry Room / B Supply Room' },
  { id: 201, mapId: 2, name: '2F Meeting Room / 2F Consul Office' },
  { id: 202, mapId: 2, name: '1F Lobby / 1F Press Room' },
  { id: 203, mapId: 2, name: 'B Archives / B Cafeteria' },
  { id: 301, mapId: 3, name: '2F Bedroom / 2F Gym' },
  { id: 302, mapId: 3, name: '1F Bar / 1F Stock Room' },
  { id: 303, mapId: 3, name: 'B Church / B Arsenal' },
  { id: 401, mapId: 4, name: '2F Armory Lockers / 2F Archives' },
  { id: 402, mapId: 4, name: '1F Workshop / 1F Ventilation' },
  { id: 403, mapId: 4, name: '1F Customs / 1F Waiting Room' },
  { id: 501, mapId: 5, name: '2F Hookah Lounge / 2F Billiards Room' },
  { id: 502, mapId: 5, name: '1F Kitchen / 1F Service' },
  { id: 503, mapId: 5, name: '2F Penthouse / 2F Theater' },
]

export const CATEGORIES = [
  { id: 1, name: 'Rush', side: 'Attack' },
  { id: 2, name: 'Default', side: 'Attack' },
  { id: 3, name: 'Fast plant', side: 'Attack' },
  { id: 4, name: 'Anchoring', side: 'Defense' },
  { id: 5, name: 'Deep roam', side: 'Defense' },
  { id: 6, name: 'Power Position Hold', side: 'Defense' },
]

export const OPERATORS = [
  { id: 1, name: 'Ash', side: 'Attack' },
  { id: 2, name: 'Nokk', side: 'Attack' },
  { id: 3, name: 'Buck', side: 'Attack' },
  { id: 4, name: 'Thermite', side: 'Attack' },
  { id: 5, name: 'Doc', side: 'Defense' },
  { id: 6, name: 'Lesion', side: 'Defense' },
  { id: 7, name: 'Azami', side: 'Defense' },
  { id: 8, name: 'Skopos', side: 'Defense' },
]

interface SeedStrat {
  id: number
  name: string
  videoUrl: string
  description: string
  mapId: number
  bombsiteId: number | null
  side: 'Attack' | 'Defense'
  categoryIds: number[]
  operatorIds: number[]
}

export const STRATS: SeedStrat[] = [
  {
    id: 1,
    name: 'Cool Ash Rush',
    videoUrl: 'https://www.youtube.com/watch?v=dQw4w9WgXx4',
    description: 'Ash rushes Hookah Lounge and opens Billiards for the plant.',
    mapId: 5,
    bombsiteId: 501,
    side: 'Attack',
    categoryIds: [1],
    operatorIds: [1],
  },
  {
    id: 2,
    name: 'Thermite breach',
    videoUrl: 'https://www.tiktok.com/@siegecoach/video/7291234567890123456',
    description: 'Thermite opens the Consul Office wall for a fast plant.',
    mapId: 2,
    bombsiteId: 201,
    side: 'Attack',
    categoryIds: [3],
    operatorIds: [4],
  },
  {
    id: 3,
    name: 'Lesion 2f setup',
    videoUrl: 'https://cdn.example.com/videos/lesion-setup.mp4',
    description: 'Lesion anchors Bedroom with gu mines on the stairs.',
    mapId: 3,
    bombsiteId: 301,
    side: 'Defense',
    categoryIds: [6],
    operatorIds: [6],
  },
  {
    id: 4,
    name: 'Basement anchor setup',
    videoUrl: 'https://vimeo.com/987654321',
    description: 'Doc anchors Laundry with the team holding Supply.',
    mapId: 1,
    bombsiteId: 104,
    side: 'Defense',
    categoryIds: [4],
    operatorIds: [5],
  },
  {
    id: 5,
    name: 'Dorms default',
    videoUrl: 'https://www.youtube.com/watch?v=aBc123xYz90',
    description: 'Nokk walks into Main Dorms and clears floor by floor.',
    mapId: 1,
    bombsiteId: 101,
    side: 'Attack',
    categoryIds: [2],
    operatorIds: [2],
  },
]

const mapOf = (id: number) => {
  const map = MAPS.find((m) => m.id === id)
  if (!map) throw new Error(`Unknown map ${id}`)
  return map
}
const bombsiteOf = (id: number | null) => BOMBSITES.find((b) => b.id === id) ?? null
const categoriesOf = (ids: number[]) => CATEGORIES.filter((c) => ids.includes(c.id))
const operatorsOf = (ids: number[]) => OPERATORS.filter((o) => ids.includes(o.id))

function toSummary(s: SeedStrat) {
  return {
    id: s.id,
    name: s.name,
    videoUrl: s.videoUrl,
    description: s.description,
    map: mapOf(s.mapId),
    bombsite: bombsiteOf(s.bombsiteId),
    side: s.side,
    categories: categoriesOf(s.categoryIds),
    operators: operatorsOf(s.operatorIds),
  }
}

function toDetail(s: SeedStrat) {
  const { side: _side, ...detail } = toSummary(s)
  return detail
}

export interface CapturedSubmissions {
  strats: Record<string, unknown>[]
  categories: Record<string, unknown>[]
}

const envelope = (data: unknown, status = 200) => ({
  status,
  data,
  message: null,
  error: null,
})
const notFound = (error: string) => ({ status: 404, data: null, message: null, error })

async function installApiMock(page: Page): Promise<CapturedSubmissions> {
  const captured: CapturedSubmissions = { strats: [], categories: [] }
  let nextSubmissionId = 1000

  // A registered service worker would serve cached API responses while
  // "offline" and make offline tests non-deterministic — block it.
  await page.context().route(/\/(dev-)?sw\.js(\?|$)/, (route) => route.abort())

  // Note: '**/api/**' would also match the frontend source file
  // /src/api/client.ts in dev mode — only paths rooted at /api are mocked.
  // The mock is registered on the CONTEXT (not the page): page.route
  // handlers are lost across page.reload(), context.route handlers are not.
  await page.context().route('**/api/**', async (route) => {
    const request = route.request()
    const path = new URL(request.url()).pathname
    if (!path.startsWith('/api/')) return route.fallback()
    const respond = (body: unknown, status = 200) =>
      route.fulfill({ status, contentType: 'application/json', body: JSON.stringify(body) })

    if (request.method() === 'GET' && path === '/api/maps') {
      return respond(envelope(MAPS))
    }
    const mapMatch = path.match(/^\/api\/maps\/(\d+)$/)
    if (request.method() === 'GET' && mapMatch) {
      const map = MAPS.find((m) => m.id === Number(mapMatch[1]))
      return map
        ? respond(envelope({ ...map, bombsites: BOMBSITES.filter((b) => b.mapId === map.id) }))
        : respond(notFound('Map not found'), 404)
    }
    if (request.method() === 'GET' && path === '/api/categories') {
      return respond(envelope(CATEGORIES))
    }
    if (request.method() === 'GET' && path === '/api/operators') {
      return respond(envelope(OPERATORS))
    }
    if (request.method() === 'GET' && path === '/api/strats/summary') {
      return respond(envelope(STRATS.map(toSummary)))
    }
    const stratMatch = path.match(/^\/api\/strats\/(\d+)$/)
    if (request.method() === 'GET' && stratMatch) {
      const strat = STRATS.find((s) => s.id === Number(stratMatch[1]))
      return strat ? respond(envelope(toDetail(strat))) : respond(notFound('Strategy not found'), 404)
    }
    if (request.method() === 'POST' && path === '/api/submissions/strats') {
      captured.strats.push(request.postDataJSON() as Record<string, unknown>)
      return respond(envelope({ submissionId: nextSubmissionId++ }), 201)
    }
    if (request.method() === 'POST' && path === '/api/submissions/categories') {
      captured.categories.push(request.postDataJSON() as Record<string, unknown>)
      return respond(envelope({ submissionId: nextSubmissionId++ }), 201)
    }
    return route.abort()
  })

  // Deterministic stubs for device capabilities (Objective #6 assertions).
  await page.addInitScript(() => {
    const g = globalThis as unknown as Record<string, unknown>
    const nav = navigator as unknown as Record<string, unknown>
    g.__vibrations = [] as unknown[]
    nav.vibrate = (pattern: unknown) => {
      ;(g.__vibrations as unknown[]).push(pattern)
      return true
    }

    g.__wakeLockRequests = 0
    g.__wakeLockReleases = 0
    Object.defineProperty(nav, 'wakeLock', {
      configurable: true,
      value: {
        request: async () => {
          ;(g.__wakeLockRequests as number)++
          return {
            release: async () => {
              ;(g.__wakeLockReleases as number)++
            },
          }
        },
      },
    })
  })

  return captured
}

// The mock is installed by overriding the built-in `page` fixture, so EVERY
// test is mocked automatically — even tests that never reference `api`
// (a fixture only runs when the test declares it, which silently left
// unmocked tests hitting the real proxy before).
const capturedByPage = new WeakMap<Page, CapturedSubmissions>()

export const test = base.extend<{ api: CapturedSubmissions }>({
  page: async ({ page }, use) => {
    const captured = await installApiMock(page)
    capturedByPage.set(page, captured)
    await use(page)
  },
  api: async ({ page }, use) => {
    await use(capturedByPage.get(page) ?? { strats: [], categories: [] })
  },
})

/**
 * Simulates losing the connection: API requests start failing (the mock is
 * shadowed by an abort route, which wins because routes are matched in
 * reverse registration order) and the browser reports itself offline.
 */
export async function cutApiAndGoOffline(page: Page): Promise<void> {
  await page.context().route('**/api/**', (route) => {
    const path = new URL(route.request().url()).pathname
    if (!path.startsWith('/api/')) return route.fallback()
    return route.abort()
  })
  await page.context().setOffline(true)
}

/**
 * Route components are lazy-loaded: a lazy chunk cannot be fetched while
 * offline, so router.push to a never-visited route is rejected and the URL
 * never changes. Warming must happen through CLIENT-SIDE navigation (a full
 * page load resets the JS module map and dev chunks are served no-cache, so
 * goto-based warming does not help). This helper opens a strategy once
 * (warms the shared /strats/:id chunk and stores a local copy) and visits
 * the offline overview once (warms its chunk), ending on /offline.
 */
export async function warmDetailAndOfflineChunks(page: Page): Promise<void> {
  await page.goto('/')
  await expect(page.locator('.map-tile')).toHaveCount(5)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.locator('a.strat-card', { hasText: 'Cool Ash Rush' }).click()
  await expect(page).toHaveURL(/\/strats\/1$/)
  await page.getByRole('link', { name: 'Offline' }).click()
  await expect(page).toHaveURL(/\/offline$/)
}

export { expect }

/** Waits until a strategy copy is actually committed to IndexedDB. */
export async function waitForOfflineSave(page: Page, id: number): Promise<void> {
  await expect
    .poll(() =>
      page.evaluate(
        (stratId) =>
          new Promise<boolean>((resolve) => {
            try {
              const request = indexedDB.open('mapify-offline')
              request.onsuccess = () => {
                const db = request.result
                const tx = db.transaction('strats', 'readonly')
                const get = tx.objectStore('strats').get(stratId)
                get.onsuccess = () => {
                  db.close()
                  resolve(get.result !== undefined)
                }
                get.onerror = () => {
                  db.close()
                  resolve(false)
                }
              }
              request.onerror = () => resolve(false)
            } catch {
              resolve(false)
            }
          }),
        id,
      ),
    )
    .toBe(true)
}