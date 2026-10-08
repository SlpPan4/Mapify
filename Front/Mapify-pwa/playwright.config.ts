import process from 'node:process'
import { defineConfig, devices } from '@playwright/test'

/**
 * The tests are fully self-contained: they mock the backend in the browser,
 * so no Docker / backend is required. Playwright serves the production build
 * on port 4173 (the same port as the Docker setup); if something is already
 * listening there (e.g. the Docker container), it is reused.
 */
export default defineConfig({
  testDir: './e2e',
  timeout: 30 * 1000,
  expect: { timeout: 5000 },
  /* Tests are independent (fresh context + fresh mock state per test). */
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: process.env.CI ? 'github' : 'list',

  use: {
    actionTimeout: 0,
    baseURL: 'http://localhost:4173',
    trace: 'on-first-retry',
    headless: true, // pass --headed to override
  },

  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'mobile-chrome', use: { ...devices['Pixel 5'] } }, // Story 13
  ],

  webServer: {
    command: 'npm run build-only && npm run preview',
    port: 4173,
    reuseExistingServer: true,
    timeout: 180 * 1000,
  },
})