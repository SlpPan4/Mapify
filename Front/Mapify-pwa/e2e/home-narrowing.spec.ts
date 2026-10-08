import { test, expect } from './fixtures.js'

/**
 * Stories 1–6, 8, 12: the "30 seconds" funnel on `/` —
 * map → bombsite → side → category/operator → name search,
 * empty results, remembered context, query-param sharing.
 */

const stratCard = (page: import('@playwright/test').Page, name: string) =>
  page.locator('a.strat-card', { hasText: name })

async function openHome(page: import('@playwright/test').Page) {
  await page.goto('/')
  // The map grid renders as soon as the (mocked) reference data has loaded.
  await expect(page.locator('.map-tile')).toHaveCount(5)
}

// Rule 1.1 — no map selected: the full library across all maps is visible.
test('without a map the full library is visible', async ({ page }) => {
  await openHome(page)
  await expect(page.locator('a.strat-card')).toHaveCount(5)
})

// Rule 1.1 — selecting a map reduces the visible strategies to that map only.
test('selecting a map narrows the library to that map', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await expect(stratCard(page, 'Cool Ash Rush')).toBeVisible()
  await expect(stratCard(page, 'Thermite breach')).toHaveCount(0)
  await expect(stratCard(page, 'Lesion 2f setup')).toHaveCount(0)
})

// Rule 1.2 — only official maps from the system can be selected.
test('the map overview contains only the official seeded maps', async ({ page }) => {
  await openHome(page)
  await expect(page.locator('.map-tile')).toHaveText([
    'Oregon',
    'Consulate',
    'Clubhouse',
    'Border',
    'Coastline',
  ])
})

// Rule 2.1 — a bombsite can only be chosen after its map is chosen.
test('the bombsite choice is unavailable until a map is picked', async ({ page }) => {
  await openHome(page)
  await expect(page.getByLabel('Bombsite')).toHaveCount(0)
  // On the library view the control exists but stays disabled.
  await page.goto('/strats')
  await expect(page.getByLabel('Bombsite')).toBeDisabled()
  await expect(page.getByLabel('Bombsite')).toContainText('Pick a map first')
})

// Rule 2.1 — the offered bombsite options belong to the chosen map.
test('a map offers only its own bombsites', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Oregon' }).click()
  await expect(page.getByLabel('Bombsite').locator('option')).toHaveText([
    'All sites',
    '2F Main Dorms Hall / 2F Kids Dormitory',
    '1F Dining Hall / 1F Kitchen',
    '1F Meeting Hall / 1F Kitchen',
    'B Laundry Room / B Supply Room',
  ])
})

// Rule 2.2 — choosing a bombsite narrows to that site; leaving it unchosen
// shows all strategies of the map.
test('choosing a bombsite narrows to the site, clearing it shows the whole map', async ({
  page,
}) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Oregon' }).click()
  await page
    .getByLabel('Bombsite')
    .selectOption({ label: 'B Laundry Room / B Supply Room' })
  await expect(stratCard(page, 'Basement anchor setup')).toBeVisible()
  await expect(stratCard(page, 'Dorms default')).toHaveCount(0)

  await page.getByLabel('Bombsite').selectOption('')
  await expect(stratCard(page, 'Dorms default')).toBeVisible()
  await expect(stratCard(page, 'Basement anchor setup')).toBeVisible()
})

// Rule 3.1 — choosing a side limits categories and operators to that side.
test('side choice constrains the offered categories and operators', async ({ page }) => {
  await page.goto('/strats')
  await page.getByRole('button', { name: 'Attack', exact: true }).click()
  await expect(page.getByLabel('Category').locator('option')).toHaveText([
    'All categories',
    'Rush',
    'Default',
    'Fast plant',
  ])
  await expect(page.getByLabel('Operator').locator('option')).toHaveText([
    'All operators',
    'Ash',
    'Nokk',
    'Buck',
    'Thermite',
  ])

  await page.getByRole('button', { name: 'Defense', exact: true }).click()
  await expect(page.getByLabel('Category').locator('option')).toHaveText([
    'All categories',
    'Anchoring',
    'Deep roam',
    'Power Position Hold',
  ])
  await expect(page.getByLabel('Operator').locator('option')).toHaveText([
    'All operators',
    'Doc',
    'Lesion',
    'Azami',
    'Skopos',
  ])
})

// Rule 3.2 — changing the side discards category/operator of the other side.
test('switching sides discards cross-side choices', async ({ page }) => {
  await page.goto('/strats')
  await page.getByRole('button', { name: 'Attack', exact: true }).click()
  await page.getByLabel('Category').selectOption({ label: 'Rush' })
  await page.getByLabel('Operator').selectOption({ label: 'Thermite' })
  await page.getByRole('button', { name: 'Defense', exact: true }).click()
  await expect(page.getByLabel('Category')).toHaveValue('')
  await expect(page.getByLabel('Operator')).toHaveValue('')
})

// Rule 4.1 — choosing an operator shows only strategies involving that operator.
test('operator filter shows only strategies with that operator', async ({ page }) => {
  await page.goto('/strats')
  await page.getByLabel('Operator').selectOption({ label: 'Thermite' })
  await expect(stratCard(page, 'Thermite breach')).toBeVisible()
  await expect(stratCard(page, 'Lesion 2f setup')).toHaveCount(0)
})

// Rule 4.2 — the operator choice combines with all other active choices.
test('operator combines with the map choice', async ({ page }) => {
  await page.goto('/?map=5&operator=1')
  await expect(stratCard(page, 'Cool Ash Rush')).toBeVisible()
  await expect(page.locator('a.strat-card')).toHaveCount(1)

  // No Consulate strategy involves Ash.
  await page.goto('/?map=2&operator=1')
  await expect(page.locator('.empty-state')).toContainText(
    'No strategy matches your current choices',
  )
})

// Rule 5.1 — name search matches partial names, case-insensitively.
test('name search is partial and case-insensitive', async ({ page }) => {
  await page.goto('/strats')
  await page.getByLabel('Search by name').fill('ash')
  await expect(stratCard(page, 'Cool Ash Rush')).toBeVisible()
  await expect(page.locator('a.strat-card')).toHaveCount(1)

  await page.getByLabel('Search by name').fill('THERMITE')
  await expect(stratCard(page, 'Thermite breach')).toBeVisible()
})

// Rule 5.2 — name search works on top of the other choices, not instead of them.
test('search combines with the active map choice', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Clubhouse' }).click()
  await page.getByLabel('Search by name').fill('setup')
  await expect(stratCard(page, 'Lesion 2f setup')).toBeVisible()

  // "Cool Ash Rush" exists, but not on Border.
  await page.locator('.map-tile', { hasText: 'Border' }).click()
  await page.getByLabel('Search by name').fill('ash')
  await expect(page.locator('.empty-state')).toContainText(
    'No strategy matches your current choices',
  )
})

// Rule 6.1 — from opening the app to an opened strategy in at most four decisions.
test('map, side and strategy are reached in three decisions', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.getByRole('button', { name: 'Attack', exact: true }).click()
  await stratCard(page, 'Cool Ash Rush').click()
  await expect(page).toHaveURL(/\/strats\/1$/)
  await expect(page.locator('h1.page-title')).toHaveText('Cool Ash Rush')
})

// Rule 8.1 — empty results say so and offer a way to broaden the search.
test('empty results offer clearing the choices', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.getByLabel('Operator').selectOption({ label: 'Skopos' })
  await expect(page.locator('.empty-state')).toContainText(
    'No strategy matches your current choices',
  )

  await page.getByRole('button', { name: 'Clear all choices' }).click()
  await expect(page.locator('a.strat-card')).toHaveCount(5)
})

// Rule 8.2 — the empty state leads to the constructor with the map pre-filled.
test('empty results offer proposing the strategy with the map pre-filled', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.getByLabel('Operator').selectOption({ label: 'Skopos' })

  await page.getByRole('button', { name: 'Propose this strategy' }).click()
  await expect(page).toHaveURL(/\/constructor$/)
  await expect(page.locator('.wizard-step.current')).toHaveText('2. Bombsite')
  await expect(page.locator('.map-tile').first()).toContainText('2F Hookah Lounge')
})

// Rule 12.1 — the most recently used map and side are restored on the next visit.
test('the last used map and side are remembered across visits', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Oregon' }).click()
  await page.getByRole('button', { name: 'Attack', exact: true }).click()

  await page.reload()
  await expect(page.locator('.map-tile.active')).toHaveText('Oregon')
  await expect(page.getByRole('button', { name: 'Attack', exact: true })).toHaveClass(
    /active-attack/,
  )
  await expect(stratCard(page, 'Dorms default')).toBeVisible()
  await expect(stratCard(page, 'Basement anchor setup')).toHaveCount(0)
})

// Rule 12.2 — the remembered context can be discarded.
test('clearing the choices also forgets the remembered context', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Oregon' }).click()
  await page.getByRole('button', { name: 'Attack', exact: true }).click()

  await page.getByRole('button', { name: 'Clear all' }).click()
  await page.reload()
  await expect(page.locator('.map-tile.active')).toHaveCount(0)
  await expect(page.locator('a.strat-card')).toHaveCount(5)
})

// Routing: active choices are mirrored into query params for sharing.
test('narrowing is mirrored into the URL query', async ({ page }) => {
  await openHome(page)
  await page.locator('.map-tile', { hasText: 'Oregon' }).click()
  await expect(page).toHaveURL(/[?&]map=1/)

  await page.getByLabel('Search by name').fill('ash')
  await expect(page).toHaveURL(/[?&]q=ash/)
})