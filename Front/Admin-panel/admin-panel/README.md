# Mapify Admin Panel

Одностраничное приложение (SPA) на Vue 3 для модерации контента Mapify: заявки на стратегии/категории (approve/reject), управление стратегиями и категориями, просмотр оперативников и карт.

## Стек

- Vue 3 (`<script setup>`, Composition API) + Vue Router 4
- Vite 8, Tailwind CSS v4
- HTTP — собственная обёртка над `fetch` (`src/api/client.js`), заголовок `X-Api-Key` на каждый запрос

## Запуск

```bash
npm install
npm run dev      # http://localhost:5173
npm run build    # production-сборка в dist/
```

## Настройка

Скопируй `.env.example` в `.env`:

- `VITE_API_KEY` — ключ админки, должен совпадать с `AdminApi:Key` бэкенда (dev-дефолт `mapify-dev-admin-key`). Vite подхватывает `VITE_*` только при старте — после смены ключа нужен перезапуск.
- `VITE_API_BASE_URL` — базовый URL бэкенда (по умолчанию `http://localhost:5000/`).

В docker-compose оба значения задаются автоматически из корневого `.env` (`ADMIN_API_KEY`) — см. корневой `README.md`.

## Поток модерации

Пользователи (например, через Discord-бота, команду `/strat-submit`) отправляют стратегии на публичный эндпоинт `POST /api/submissions/strats`. Заявки появляются на странице `/pending`; после approve они переносятся в публичные таблицы и становятся видны клиентам, после reject — удаляются.

Подробная архитектура, соглашения по коду и правила для агентов — в `../AGENTS.md`. API бэкенда описан в `Back/MapifyBackend/API_DOCUMENTATION.md`.
