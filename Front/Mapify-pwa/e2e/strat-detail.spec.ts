import { test, expect } from './fixtures.js'

/**
 * Story 7: viewing one strategy — the embedded video, the written plan,
 * and the map / side / categories / operators metadata.
 */

// Rule 7.1 — a YouTube link plays inside the strategy view.
test('a YouTube strategy embeds its video', async ({ page }) => {
  await page.goto('/strats/1')
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await expect(page.locator('.video-wrapper iframe')).toHaveAttribute(
    'src',
    /youtube\.com\/embed\/dQw4w9WgXx4$/,
  )
})

// Rule 7.1 — a TikTok link plays inside the strategy view.
test('a TikTok strategy embeds its video', async ({ page }) => {
  await page.goto('/strats/2')
  await expect(page.locator('h1.page-title')).toHaveText('Thermite breach')
  await expect(page.locator('.video-wrapper iframe')).toHaveAttribute(
    'src',
    /tiktok\.com\/embed\/v2\/7291234567890123456$/,
  )
})

// A direct video file is played with a native <video> element.
test('a direct video file renders a native player', async ({ page }) => {
  await page.goto('/strats/3')
  await expect(page.locator('.video-wrapper video')).toHaveAttribute('src', /lesion-setup\.mp4$/)
})

// Rule 7.2 — an unsupported platform shows an external link, everything
// else (plan, metadata) stays visible.
test('an unsupported video platform falls back to an external link', async ({ page }) => {
  await page.goto('/strats/4')
  await expect(page.locator('.video-fallback')).toContainText('Video hosted on vimeo.com')
  await expect(
    page.locator('.video-fallback a', { hasText: 'Watch externally' }),
  ).toHaveAttribute('href', /vimeo\.com/)

  // The rest of the information remains visible.
  await expect(page.locator('.detail-meta')).toContainText('Oregon')
  await expect(page.locator('.detail-meta')).toContainText('B Laundry Room / B Supply Room')
  await expect(page.locator('.detail-meta')).toContainText('Anchoring')
  await expect(page.locator('.detail-meta .badge')).toHaveText('Defense')
  await expect(page.locator('.detail-meta .op-badge[title="Doc"]')).toBeVisible()
  await expect(page.locator('.card')).toContainText('Doc anchors Laundry with the team')
})

// Rule 7.2 — map, side, categories, operators and the written plan are
// shown alongside the video.
test('the strategy metadata and the plan are shown next to the video', async ({ page }) => {
  await page.goto('/strats/1')
  await expect(page.locator('.detail-meta')).toContainText('Coastline')
  await expect(page.locator('.detail-meta')).toContainText('2F Hookah Lounge / 2F Billiards Room')
  await expect(page.locator('.detail-meta')).toContainText('Rush')
  await expect(page.locator('.detail-meta .badge')).toHaveText('Attack')
  await expect(page.locator('.detail-meta .op-badge[title="Ash"]')).toBeVisible()
  await expect(page.locator('.card')).toContainText('Ash rushes Hookah Lounge')
})

// Story: a strategy can be handed to a teammate as a QR code.
test('the strategy view offers QR sharing of its link', async ({ page }) => {
  await page.goto('/strats/1')
  await page.getByRole('button', { name: 'Share this strategy' }).click()
  const dialog = page.getByRole('dialog', { name: 'Share via QR code' })
  await expect(dialog).toBeVisible()
  await expect(dialog.locator('canvas')).toBeVisible()
  await expect(dialog).toContainText('/strats/1')
})