import { test, expect, cutApiAndGoOffline, warmDetailAndOfflineChunks, waitForOfflineSave } from './fixtures.js'

/**
 * Story 16: offline access — strategies fully opened while online are kept
 * on the device and stay available (video excepted) without a connection.
 *
 * Note: after context.setOffline(true) Chromium refuses full page loads,
 * so every transition below happens through in-app (router-link) navigation,
 * and the route chunks are warmed up first (see warmDetailAndOfflineChunks).
 */

// Rule 16.1 — fully opening a strategy saves a copy on the device; the
// offline overview lists it.
test('opening a strategy online saves it on the device', async ({ page }) => {
  await page.goto('/strats/1')
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await waitForOfflineSave(page, 1)

  await page.goto('/offline')
  await expect(page.locator('h1.page-title')).toHaveText('Saved on this device')
  await expect(page.locator('a.strat-card', { hasText: 'Cool Ash Rush' })).toBeVisible()
})

// Rule 16.1 — merely seeing a strategy in the results list does not save it.
test('strategies only seen in the list are not saved offline', async ({ page }) => {
  await page.goto('/?map=2')
  await expect(page.locator('a.strat-card', { hasText: 'Thermite breach' })).toBeVisible()

  await page.goto('/offline')
  await expect(page.locator('.empty-state')).toContainText(
    'No strategies are saved on this device yet',
  )
})

// Rules 16.2 + 16.3 — offline, exactly the previously viewed strategies
// open; reopening them (even after navigating away) still works; the video
// is replaced by an offline notice.
test('a saved strategy stays fully readable without a connection', async ({ page }) => {
  await warmDetailAndOfflineChunks(page) // ends on /offline; "Cool Ash Rush" is saved

  await cutApiAndGoOffline(page)
  await expect(page.locator('.banner-offline')).toBeVisible()

  await page.locator('a.strat-card', { hasText: 'Cool Ash Rush' }).click()
  await expect(page).toHaveURL(/\/strats\/1$/)
  await waitForOfflineSave(page, 1)
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await expect(page.locator('.page-subtitle')).toContainText('copy saved on this device')
  // The written plan, map, side, categories and operators are available.
  await expect(page.locator('.card')).toContainText('Ash rushes Hookah Lounge')
  await expect(page.locator('.detail-meta')).toContainText('Coastline')
  await expect(page.locator('.detail-meta .badge')).toHaveText('Attack')
  await expect(page.locator('.detail-meta .op-badge[title="Ash"]')).toBeVisible()
  // The video needs a connection and says so.
  await expect(page.locator('.offline-notice')).toContainText('needs an internet connection')
  await expect(page.locator('iframe')).toHaveCount(0)
})

// Rule 16.2 — a strategy never fully viewed before is unavailable offline,
// and the notice offers the locally stored strategies instead.
test('a never-opened strategy shows an offline notice', async ({ page }) => {
  await warmDetailAndOfflineChunks(page) // ends on /offline; "Cool Ash Rush" is saved
  await page.getByRole('link', { name: 'Home' }).click()
  await expect(page).toHaveURL(/\/$/)

  await cutApiAndGoOffline(page)
  await expect(page.locator('.banner-offline')).toBeVisible()

  // Client-side navigation to a strategy that was never fully opened.
  await page.locator('.map-tile', { hasText: 'Consulate' }).click()
  await page.locator('a.strat-card', { hasText: 'Thermite breach' }).click()
  await expect(page).toHaveURL(/\/strats\/2$/)
  await expect(page.locator('.offline-notice')).toContainText(
    'This strategy was never opened before',
  )

  // The notice offers the strategies saved on this device.
  await page.locator('.offline-notice a', { hasText: 'saved on this device' }).click()
  await expect(page).toHaveURL(/\/offline$/)
})

// Without a connection the app points at the saved strategies instead of
// leaving the user on a dead end.
test('offline notices lead to the strategies saved on this device', async ({ page }) => {
  await warmDetailAndOfflineChunks(page) // ends on /offline; "Cool Ash Rush" is saved

  await cutApiAndGoOffline(page)
  await page.locator('a.strat-card', { hasText: 'Cool Ash Rush' }).click()
  // The video placeholder explains the situation and links to saved content.
  await expect(page.locator('.offline-notice')).toContainText('needs an internet connection')
  await page.locator('.offline-notice a', { hasText: 'saved on this device' }).click()
  await expect(page).toHaveURL(/\/offline$/)
})

// Submission requires a connection; the forms explain and disable themselves.
test('submission forms are disabled while offline', async ({ page }) => {
  await page.goto('/submit')
  await expect(page.locator('#sub-map option')).toHaveCount(6)

  await cutApiAndGoOffline(page)
  await expect(page.locator('.banner-offline')).toBeVisible()
  await expect(page.locator('.offline-notice')).toContainText(
    'Submissions need an internet connection',
  )
  await expect(page.getByRole('button', { name: 'Submit strategy for review' })).toBeDisabled()
  await expect(page.getByRole('button', { name: 'Propose category' })).toBeDisabled()
})