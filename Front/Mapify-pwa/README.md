# Mapify PWA

The public Mapify web app: a Rainbow Six Siege strategy library built with Vue 3, TypeScript, Pinia and vue-router. Installable as a PWA; strategies you fully open are kept on your device for offline use.

## Requirements

- Node.js `^22.18.0 || >=24.12.0`
- The Mapify backend running on `http://localhost:5000` (or reachable via the `VITE_API_PROXY_TARGET` env var)

## Development

```sh
npm install
npm run dev
```

The dev server runs on `http://localhost:5173` and proxies `/api/...` to `http://localhost:5000`. Override the proxy target with the `VITE_API_PROXY_TARGET` environment variable. The service worker is enabled in dev mode, so PWA behavior can be checked without a production build.

## Build and preview

```sh
npm run build      # type-check + production build into dist/
npm run preview    # serves dist/ on http://localhost:4173 (also proxies /api)
```

## Lint

```sh
npm run lint
```

## Docker

From the `Mapify/` directory:

```sh
docker compose build pwa
docker compose up backend pwa
```

The app is then available on `http://localhost:4173`. The container builds the production bundle and serves it with `vite preview`; `VITE_API_PROXY_TARGET=http://backend:5000` (set in `docker-compose.yaml`) routes API calls to the backend service over the compose network.

## Notes

- End-to-end tests use Playwright: `npm run test:e2e` (requires `npx playwright install` and a running app).
- All API calls go through `src/api/client.ts`; offline copies of strategies live in IndexedDB (`src/offline/db.ts`).
