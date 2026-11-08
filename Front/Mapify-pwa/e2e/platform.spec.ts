import { test, expect, waitForOfflineSave } from './fixtures.js'

/**
 * Objective #6: device/platform capabilities — vibration, wake lock,
 * notifications (granted + denied), QR sharing and the PWA manifest.
 * The capabilities are stubbed deterministically in `installApiMock`;
 * assertions check user-visible behavior, not internals.
 */

// Vibration — picking a map pulses the haptic API (stubbed).
test('picking a map triggers haptic feedback', async ({ page }) => {
  await page.goto('/')
  await expect(page.locator('.map-tile')).toHaveCount(5)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await expect
    .poll(() =>
      page.evaluate(() => (window as unknown as { __vibrations: unknown[] }).__vibrations.length),
    )
    .toBeGreaterThan(0)
})

// Wake lock — the strategy view keeps the screen awake while the plan is open.
test('the strategy view requests a screen wake lock', async ({ page }) => {
  await page.goto('/strats/1')
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await expect
    .poll(() =>
      page.evaluate(
        () => (window as unknown as { __wakeLockRequests: number }).__wakeLockRequests,
      ),
    )
    .toBe(1)
})

// Notifications — when permission is already granted, saving a freshly
// viewed strategy confirms via a local notification. The permission state is
// stubbed (grantPermissions is unreliable in headless Chromium) so the test
// is deterministic on any machine.
test('saving a strategy for offline use confirms via a notification', async ({ page }) => {
  await page.addInitScript(() => {
    const w = window as unknown as Record<string, unknown>
    w.__notifications = [] as { title: string; body?: string }[]
    const Original = window.Notification
    const Fake = function (this: unknown, title: string, options?: NotificationOptions) {
      ;(w.__notifications as { title: string; body?: string }[]).push({ title, body: options?.body })
      try {
        return new Original(title, options)
      } catch {
        return {}
      }
    } as unknown as typeof Notification
    Object.setPrototypeOf(Fake, Original)
    Object.defineProperty(Fake, 'permission', { get: () => 'granted' })
    window.Notification = Fake
  })

  await page.goto('/strats/1')
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await expect
    .poll(() =>
      page.evaluate(
        () => (window as unknown as { __notifications: unknown[] }).__notifications.length,
      ),
    )
    .toBe(1)
  const notification = await page.evaluate(
    () =>
      (window as unknown as { __notifications: { title: string; body?: string }[] })
        .__notifications[0],
  )
  expect(notification.title).toBe('Mapify')
  expect(notification.body).toContain('Cool Ash Rush')
})

// Permission denial — with notifications denied nothing is requested or
// shown, and the app keeps working (the strategy is still saved offline).
test('denied notification permission never breaks the flow', async ({ page }) => {
  await page.addInitScript(() => {
    const w = window as unknown as Record<string, unknown>
    w.__notificationRequests = 0
    Object.defineProperty(window.Notification, 'permission', {
      get: () => 'denied',
      configurable: true,
    })
    const originalRequest = window.Notification.requestPermission.bind(window.Notification)
    Object.defineProperty(window.Notification, 'requestPermission', {
      get: () => async () => {
        ;(w.__notificationRequests as number)++
        return originalRequest()
      },
      configurable: true,
    })
  })

  await page.goto('/strats/1')
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
  await waitForOfflineSave(page, 1)

  // The strategy was still saved for offline use.
  await page.goto('/offline')
  await expect(page.locator('a.strat-card', { hasText: 'Cool Ash Rush' })).toBeVisible()
  // A denied permission does not cause a permission re-request loop.
  const requests = await page.evaluate(
    () => (window as unknown as { __notificationRequests: number }).__notificationRequests,
  )
  expect(requests).toBe(0)
})

// PWA — the app exposes an installable manifest with its own icon set.
test('the app exposes an installable PWA manifest', async ({ page }) => {
  await page.goto('/')
  const manifestLink = page.locator('link[rel="manifest"]')
  await expect(manifestLink).toHaveAttribute('href', /^\//)
  const manifestHref = await manifestLink.getAttribute('href')
  const manifest = await page.evaluate(async (href) => {
    const response = await fetch(href!)
    return response.json()
  }, manifestHref)
  expect(manifest.name).toBe('Mapify')
  expect(manifest.display).toBe('standalone')
  expect(manifest.icons.length).toBeGreaterThanOrEqual(2)
})