import { test, expect } from './fixtures.js'

/**
 * Stories 10, 11, 14: submitting strategies and category proposals —
 * required fields (name + map + bombsite + video link), side consistency,
 * review queue, and the sketch-less path.
 */

const stratForm = (page: import('@playwright/test').Page) =>
  page.locator('section.card').filter({ has: page.locator('#sub-name') })
const categoryForm = (page: import('@playwright/test').Page) =>
  page.locator('section.card').filter({ has: page.locator('#cat-name') })

async function openSubmit(page: import('@playwright/test').Page) {
  await page.goto('/submit')
  // The map select is populated once reference data has loaded.
  await expect(page.locator('#sub-map option')).toHaveCount(6)
}

/** Fills the form with a complete, valid strategy draft. */
async function fillValidStrategy(page: import('@playwright/test').Page) {
  await page.locator('#sub-name').fill('Nokk silent push')
  await page.locator('#sub-map').selectOption({ label: 'Oregon' })
  await page
    .locator('#sub-bombsite')
    .selectOption({ label: 'B Laundry Room / B Supply Room' })
  await page.locator('#sub-video').fill('https://www.youtube.com/watch?v=dQw4w9WgXx4')
}

// Rules 10.1 + 14.1/14.2 — a minimal submission (name + map + bombsite +
// video link, without ever touching the constructor) is accepted for review.
test('a sketch-less submission with the required fields is accepted', async ({ page, api }) => {
  await openSubmit(page)
  await fillValidStrategy(page)

  await page.getByRole('button', { name: 'Submit strategy for review' }).click()
  await expect(stratForm(page).locator('.form-success')).toContainText('submitted for review')
  expect(api.strats).toHaveLength(1)
  expect(api.strats[0]).toMatchObject({ name: 'Nokk silent push', mapId: 1, bombsiteId: 104 })
})

// Rule 10.1 — the bombsite is a required part of a submission; without it the
// form blocks submission (native constraint validation) and nothing is sent.
test('a submission without a bombsite is blocked by the form', async ({ page, api }) => {
  await openSubmit(page)
  await page.locator('#sub-name').fill('No site strat')
  await page.locator('#sub-map').selectOption({ label: 'Oregon' })
  await page.locator('#sub-video').fill('https://www.youtube.com/watch?v=dQw4w9WgXx4')

  await page.getByRole('button', { name: 'Submit strategy for review' }).click()
  // The browser reports the missing required field instead of submitting.
  await expect(page.locator('#sub-bombsite')).toHaveJSProperty(
    'validationMessage',
    'Please select an item in the list.',
  )
  expect(api.strats).toHaveLength(0)
})

// Rule 10.1 — a missing video link is rejected with a clear reason, nothing is sent.
test('a submission without a video link is rejected', async ({ page, api }) => {
  await openSubmit(page)
  await page.locator('#sub-name').fill('No video strat')
  await page.locator('#sub-map').selectOption({ label: 'Oregon' })
  await page.locator('#sub-bombsite').selectOption({ label: 'B Laundry Room / B Supply Room' })

  await page.getByRole('button', { name: 'Submit strategy for review' }).click()
  await expect(stratForm(page).locator('.form-error')).toContainText('A video link is required')
  expect(api.strats).toHaveLength(0)
})

// Rule 10.2 — categories and operators must all belong to the same side.
test('a cross-side category and operator combination is rejected before sending', async ({
  page,
  api,
}) => {
  await openSubmit(page)
  await fillValidStrategy(page)
  await page.getByRole('button', { name: 'Rush (Attack)' }).click()
  await page.getByRole('button', { name: 'Doc (Defense)' }).click()

  await page.getByRole('button', { name: 'Submit strategy for review' }).click()
  await expect(stratForm(page).locator('.form-error')).toContainText(
    'must all belong to the same side',
  )
  // Rejected client-side: the request never reaches the server.
  expect(api.strats).toHaveLength(0)
})

// Rule 10.3 — a submitted strategy is not publicly visible until approved.
test('a submitted strategy does not appear in the library until approved', async ({
  page,
  api,
}) => {
  await openSubmit(page)
  await fillValidStrategy(page)
  await page.getByRole('button', { name: 'Submit strategy for review' }).click()
  await expect(stratForm(page).locator('.form-success')).toBeVisible()
  expect(api.strats).toHaveLength(1)

  await page.goto('/strats')
  await page.getByLabel('Search by name').fill('Nokk')
  await expect(page.locator('.empty-state')).toContainText(
    'No strategy matches your current choices',
  )
})

// Rule 11.1 — a category proposal requires a name and a side.
test('a category proposal without a side is rejected with a reason', async ({ page, api }) => {
  await openSubmit(page)
  await page.locator('#cat-name').fill('Shield clear')
  await page.getByRole('button', { name: 'Propose category' }).click()
  await expect(categoryForm(page).locator('.form-error')).toContainText(
    'Please choose a side for the category',
  )
  expect(api.categories).toHaveLength(0)
})

// Rule 11.1 — a valid proposal is accepted for review.
test('a category proposal with name and side is accepted', async ({ page, api }) => {
  await openSubmit(page)
  await page.locator('#cat-name').fill('Shield clear')
  await page.locator('#cat-side').selectOption('Attack')
  await page.getByRole('button', { name: 'Propose category' }).click()

  await expect(categoryForm(page).locator('.form-success')).toContainText(
    'becomes selectable after moderator approval',
  )
  expect(api.categories).toHaveLength(1)
  expect(api.categories[0]).toMatchObject({ name: 'Shield clear', side: 'Attack' })
})

// Rule 11.2 — the proposed category is not selectable until approved.
test('a proposed category is not offered among the categories yet', async ({ page }) => {
  await openSubmit(page)
  await page.locator('#cat-name').fill('Shield clear')
  await page.locator('#cat-side').selectOption('Attack')
  await page.getByRole('button', { name: 'Propose category' }).click()
  await expect(categoryForm(page).locator('.form-success')).toBeVisible()

  await page.goto('/strats')
  const optionLabels = await page.getByLabel('Category').locator('option').allTextContents()
  expect(optionLabels).not.toContain('Shield clear')
})