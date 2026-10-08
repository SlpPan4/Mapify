import { test, expect } from './fixtures.js'

/**
 * Story 9: the strategy constructor — wizard over an official map, one of
 * that map's bombsites, then a sketch with operator icons and notes.
 */

// Rule 9.1 — the constructor always works on an official map and requires
// choosing one of that map's bombsites before sketching.
test('the wizard requires an official map and one of its bombsites', async ({ page }) => {
  await page.goto('/constructor')
  await expect(page.locator('.map-tile')).toHaveCount(5)
  // No sketch canvas before map + bombsite are chosen.
  await expect(page.locator('.sketch-canvas')).toHaveCount(0)

  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await expect(page.locator('.wizard-step.current')).toHaveText('2. Bombsite')
  await expect(page.locator('.map-tile')).toHaveText([
    '2F Hookah Lounge / 2F Billiards Room',
    '1F Kitchen / 1F Service',
    '2F Penthouse / 2F Theater',
  ])

  await page.locator('.map-tile', { hasText: '2F Hookah Lounge' }).click()
  await expect(page.locator('.wizard-step.current')).toHaveText('3. Sketch')
  // The sketch surface unlocks only after the side is picked.
  await expect(page.locator('.sketch-canvas')).toHaveCount(0)
  await page.getByRole('button', { name: 'Attack', exact: true }).click()
  await expect(page.locator('.sketch-canvas')).toBeVisible()
})

// Rule 9.2 — only operators of the strategy's side can be placed.
test('the sketch palette offers only operators of the chosen side', async ({ page }) => {
  await page.goto('/constructor')
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.locator('.map-tile', { hasText: '1F Kitchen' }).click()

  const palette = page.locator('.palette')
  // No palette at all before a side is picked.
  await expect(palette).toHaveCount(0)
  await expect(page.getByText('Pick a side to unlock the operator palette')).toBeVisible()

  await page.getByRole('button', { name: 'Defense', exact: true }).click()
  for (const name of ['Doc', 'Lesion', 'Azami', 'Skopos']) {
    await expect(palette.locator(`[title="${name}"]`)).toBeVisible()
  }
  await expect(palette.locator('[title="Ash"]')).toHaveCount(0)

  await page.getByRole('button', { name: 'Attack', exact: true }).click()
  for (const name of ['Ash', 'Nokk', 'Buck', 'Thermite']) {
    await expect(palette.locator(`[title="${name}"]`)).toBeVisible()
  }
  await expect(palette.locator('[title="Doc"]')).toHaveCount(0)
})

// Rule 9.2 — the sketch consists of operator icons and text notes on the map.
test('operators and notes can be placed on the layout and removed', async ({ page }) => {
  await page.goto('/constructor')
  await page.locator('.map-tile', { hasText: 'Coastline' }).click()
  await page.locator('.map-tile', { hasText: '2F Hookah Lounge' }).click()
  await page.getByRole('button', { name: 'Attack', exact: true }).click()

  // Place Ash at the balcony.
  await page.locator('.palette [title="Ash"]').click()
  await page.locator('.sketch-canvas').click()
  await expect(page.locator('.sketch-item[title="Ash — click to remove"]')).toBeVisible()

  // Add the note "rush through balcony" next to her.
  await page.getByRole('button', { name: '+ Note' }).click()
  await page.locator('.sketch-canvas').click({ position: { x: 120, y: 90 } })
  await page.locator('#note-text').fill('rush through balcony')
  await page.getByRole('button', { name: 'Place note' }).click()
  await expect(page.locator('.sketch-note')).toHaveText('rush through balcony')

  // Tapping a placed item removes it.
  await page.locator('.sketch-item').click()
  await expect(page.locator('.sketch-item')).toHaveCount(0)
  await expect(page.locator('.sketch-note')).toBeVisible()

  // The sketch flows into the submission.
  await page.getByRole('button', { name: 'Continue to submission' }).click()
  await expect(page.locator('h2', { hasText: 'Submit a strategy' })).toBeVisible()
})