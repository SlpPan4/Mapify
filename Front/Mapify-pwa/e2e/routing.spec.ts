import { test, expect } from './fixtures.js'

/**
 * Routing: the library route, the strategy deep link and the 404 fallback.
 */

// `/strats` — the full library as a searchable, filterable list, grouped by map.
test('the library route shows the full catalog grouped by map', async ({ page }) => {
  await page.goto('/strats')
  await expect(page.locator('h1.page-title')).toHaveText('Strategy library')
  await expect(page.locator('section.library-group')).toHaveCount(4)
  await expect(page.locator('a.strat-card')).toHaveCount(5)
  // Relaxed browsing: every filter capability is available here too.
  await page.getByLabel('Search by name').fill('lesion')
  await expect(page.locator('a.strat-card', { hasText: 'Lesion 2f setup' })).toBeVisible()
  await expect(page.locator('a.strat-card')).toHaveCount(1)
})

// `/strats/:id` — a shared deep link opens the strategy directly.
test('a strategy deep link opens the strategy directly', async ({ page }) => {
  await page.goto('/strats/3')
  await expect(page.locator('h1.page-title')).toHaveText('Lesion 2f setup')
  await expect(page.locator('.detail-meta')).toContainText('Clubhouse')
})

// Unknown URLs fall back to a 404 view with a way back.
test('unknown URLs render a 404 view with a way back home', async ({ page }) => {
  await page.goto('/no-such-route')
  await expect(page.getByRole('heading', { name: '404 — page not found' })).toBeVisible()
  await expect(page.locator('.empty-state')).toContainText('does not exist')

  await page.getByRole('link', { name: 'Back to the strategy finder' }).click()
  await expect(page).toHaveURL(/\/$/)
  await expect(page.locator('h1.page-title')).toHaveText('Find your strategy')
})