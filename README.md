# Mapify — setup guide

*English version below — [Русская версия](#mapify--мануал-по-запуску).*

Stack: ASP.NET Core (.NET 10) backend + Vue 3 (Vite) admin panel + Telegram and Discord bots. Data lives in a SQLite file.

## Quick start: Docker (one command)

All you need is Docker Desktop. **Before the first run, create a `.env` file in the repo root** (copy it from `.env.example`) and put your admin key there:

```
ADMIN_API_KEY=my-secret-key
```

The same key is used by both the backend and the admin panel (details in [Environment variables and the admin key](#environment-variables-and-the-admin-key)). Without `.env` the default dev key `mapify-dev-admin-key` is used — fine for local dev, but do not rely on it anywhere else.

Then, from the repo root:

```bash
docker compose up --build
```

Four services come up:

- **backend** — API at http://localhost:5000
- **frontend** — admin panel at http://localhost:5173
- **telegram-bot** — Telegram bot (starts only if `Front/Telegram/.env` contains a valid token, see below)
- **discord-bot** — Discord bot (starts only if `Front/Discord/.env` contains a valid token, see below)

### Telegram bot

The bot needs its own `.env`: copy `Front/Telegram/.env.example` to `Front/Telegram/.env` and put your token from @BotFather there:

```
TELEGRAM_BOT_TOKEN=your-bot-token-here
```

Inside compose the bot talks to the backend at `http://backend:5000/api/` automatically. Without `Front/Telegram/.env` the container exits immediately — the rest of the stack is unaffected. Running the bot locally without Docker works too: the same `.env` is picked up by `src/config.py`.

### Discord bot

The Discord bot needs its own `.env`: copy `Front/Discord/.env.example` to `Front/Discord/.env` and fill in the credentials from the Discord Developer Portal:

```
DISCORD_TOKEN=your-bot-token-here
DISCORD_CLIENT_ID=your-application-id-here
```

Optionally set `TRN_API_KEY` (Tracker Network API key) to enable the `/r6stats` player-stats command. Inside compose the bot talks to the backend at `http://backend:5000` automatically. Without `Front/Discord/.env` the container exits immediately — the rest of the stack is unaffected. Local run without Docker: `cd Front/Discord && npm install && npm start`.

The Discord bot has **no admin key by design**: it only reads strategies and submits new ones for review via the public `POST /api/submissions/strats` endpoint (`/strat-submit` command). Submitted strategies appear in the bot only after approval in the admin panel. Slash commands are re-registered globally on every bot start.

The admin panel code is mounted into the container, so frontend edits are picked up on the fly (hot reload). The backend database lives in the `backend-data` docker volume and survives rebuilds.

Stop: `Ctrl+C` or `docker compose down` (add `-v` if you want to wipe the database too).

### Running backend tests in Docker

```bash
docker compose --profile test run --rm backend-tests
```

(Tests also run automatically when the backend image is built — if they fail, the build fails.)

## Environment variables and the admin key

Admin endpoints and all mutating requests (POST/PUT/DELETE/PATCH) are protected by a key that the client sends in the `X-Api-Key` header. The same key must be known to both the backend and the admin panel.

### In Docker

**The `.env` file with the key is the required setup step**: create it in the **repo root** (next to `docker-compose.yaml`, copy from `.env.example`) and set:

```
ADMIN_API_KEY=my-secret-key
```

Compose picks it up automatically and passes it to both the backend (`AdminApi__Key`) and the panel (`VITE_API_KEY`). After changing it: `docker compose up --build` (or at least `docker compose restart`).

If you skip this step, `docker compose up` still works — the default dev key `mapify-dev-admin-key` is used. Never use it outside local development.

### Without Docker (local development)

Backend (terminal 1):

```bash
cd Back/MapifyBackend
dotnet run --project MapifyBackend/MapifyBackend.csproj --urls "http://localhost:5000"
```

Admin panel (terminal 2):

```bash
cd Front/Admin-panel/admin-panel
npm install   # once
npm run dev
```

The panel opens at http://localhost:5173 and talks to `http://localhost:5000`.

- The backend key in dev mode lives in `Back/MapifyBackend/MapifyBackend/appsettings.Development.json` (`AdminApi:Key`).
- The panel key lives in `Front/Admin-panel/admin-panel/.env` (create it from `.env.example`: `VITE_API_KEY=...`). If there is no `.env`, the panel uses the default dev key.

### Why an environment variable might "not work"

- **Nested ASP.NET Core settings** use a **double** underscore: `AdminApi__Key`, not `AdminApi:Key` and not `ADMIN_API_KEY`. In docker-compose, variables are interpolated from the root `.env` — the only file compose reads automatically.
- **`VITE_*` variables** are picked up by Vite only when the dev server starts / at build time. Changed a value — restart `npm run dev` or the container.
- Outside the Development environment the backend **fails at startup** if the key is missing or equals the dev key — this is deliberate, so production never comes up with an empty/default key.

Manual key check:

```bash
# without the key — 401
curl -X DELETE http://localhost:5000/api/strats/1

# with the key — ok
curl -H "X-Api-Key: mapify-dev-admin-key" http://localhost:5000/api/submissions/admin/strats
```

## Repository structure

```text
Back/MapifyBackend/       # ASP.NET Core API (+ integration tests)
Front/Admin-panel/        # admin panel (working app is in the admin-panel/ subfolder)
Front/Discord/            # Discord bot (docker-compose service discord-bot, needs Front/Discord/.env)
Front/Telegram/           # Telegram bot (docker-compose service telegram-bot, needs Front/Telegram/.env)
docker-compose.yaml       # backend + frontend + telegram-bot + discord-bot (+ test profile)
```

Details on each part — in the `AGENTS.md` files of the corresponding folders and in `Back/MapifyBackend/API_DOCUMENTATION.md`.

---

# Mapify — мануал по запуску

*[English version](#mapify--setup-guide).*

Стек: бэкенд на ASP.NET Core (.NET 10) + админ-панель на Vue 3 (Vite) + боты для Telegram и Discord. Данные — в SQLite-файле.

## Быстрый старт: Docker (одна команда)

Нужен только установленный Docker Desktop. **Перед первым запуском создай в корне репозитория файл `.env`** (скопируй из `.env.example`) и впиши туда ключ админки:

```
ADMIN_API_KEY=мой-секретный-ключ
```

Один и тот же ключ используется и бэкендом, и админ-панелью (подробности в разделе [Переменные окружения и ключ админки](#переменные-окружения-и-ключ-админки)). Без `.env` используется дефолтный dev-ключ `mapify-dev-admin-key` — для локальной разработки сойдёт, но нигде больше на него не полагайся.

Затем из корня репозитория:

```bash
docker compose up --build
```

Поднимутся четыре сервиса:

- **backend** — API на http://localhost:5000
- **frontend** — админ-панель на http://localhost:5173
- **telegram-bot** — Telegram-бот (запустится, только если в `Front/Telegram/.env` задан токен, см. ниже)
- **discord-bot** — Discord-бот (запустится, только если в `Front/Discord/.env` задан токен, см. ниже)

### Telegram-бот

Боту нужен свой `.env`: скопируй `Front/Telegram/.env.example` в `Front/Telegram/.env` и впиши туда токен от @BotFather:

```
TELEGRAM_BOT_TOKEN=токен-твоего-бота
```

Внутри compose бот автоматически ходит на бэкенд по адресу `http://backend:5000/api/`. Без `Front/Telegram/.env` контейнер сразу завершится — на остальные сервисы это не влияет. Локальный запуск без Docker тоже работает: тот же `.env` подхватывается в `src/config.py`.

### Discord-бот

Discord-боту нужен свой `.env`: скопируй `Front/Discord/.env.example` в `Front/Discord/.env` и впиши данные из Discord Developer Portal:

```
DISCORD_TOKEN=токен-твоего-бота
DISCORD_CLIENT_ID=application-id-бота
```

Опционально задай `TRN_API_KEY` (ключ Tracker Network API) — он включает команду `/r6stats` со статистикой игроков. Внутри compose бот автоматически ходит на бэкенд по адресу `http://backend:5000`. Без `Front/Discord/.env` контейнер сразу завершится — на остальные сервисы это не влияет. Локальный запуск без Docker: `cd Front/Discord && npm install && npm start`.

У Discord-бота **нет ключа админки — это by design**: он только читает стратегии и отправляет новые на модерацию через публичный `POST /api/submissions/strats` (команда `/strat-submit`). Отправленная стратегия появится в боте после одобрения в админ-панели. Slash-команды перерегистрируются глобально при каждом запуске бота.

Код админ-панели примонтирован в контейнер, поэтому правки фронта подхватываются на лету (hot reload). База бэкенда лежит в docker-volume `backend-data` и переживает пересборку.

Остановка: `Ctrl+C` или `docker compose down` (добавь `-v`, если хочешь снести и базу).

### Прогон тестов бэкенда в Docker

```bash
docker compose --profile test run --rm backend-tests
```

(Тесты также запускаются автоматически при сборке образа бэкенда — если они падают, сборка не пройдёт.)

## Переменные окружения и ключ админки

Админ-эндпоинты и все изменяющие запросы (POST/PUT/DELETE/PATCH) защищены ключом, который клиент шлёт в заголовке `X-Api-Key`. Один и тот же ключ должен знать и бэкенд, и админ-панель.

### В Docker

**Файл `.env` с ключом — обязательный шаг настройки**: создай его в **корне репозитория** (рядом с `docker-compose.yaml`, скопируй из `.env.example`) и задай:

```
ADMIN_API_KEY=мой-секретный-ключ
```

Compose сам подхватит его и передаст и в бэкенд (`AdminApi__Key`), и в админку (`VITE_API_KEY`). После изменения: `docker compose up --build` (или хотя бы `docker compose restart`).

Если пропустить этот шаг, `docker compose up` всё равно сработает — будет использован дефолтный dev-ключ `mapify-dev-admin-key`. Никогда не используй его вне локальной разработки.

### Без Docker (локальная разработка)

Бэкенд (терминал 1):

```bash
cd Back/MapifyBackend
dotnet run --project MapifyBackend/MapifyBackend.csproj --urls "http://localhost:5000"
```

Админ-панель (терминал 2):

```bash
cd Front/Admin-panel/admin-panel
npm install   # один раз
npm run dev
```

Панель откроется на http://localhost:5173 и будет ходить на `http://localhost:5000`.

- Ключ бэкенда в dev-режиме лежит в `Back/MapifyBackend/MapifyBackend/appsettings.Development.json` (`AdminApi:Key`).
- Ключ панели — в `Front/Admin-panel/admin-panel/.env` (создай по образцу `.env.example`: `VITE_API_KEY=...`). Если `.env` нет, панель использует dev-ключ по умолчанию.

### Почему переменная окружения могла «не работать»

- **Вложенные настройки ASP.NET Core** задаются через **двойное** подчёркивание: `AdminApi__Key`, а не `AdminApi:Key` и не `ADMIN_API_KEY`. В docker-compose переменные интерполируются из корневого `.env` — это единственное место, которое compose читает автоматически.
- **Переменные `VITE_*`** подхватываются Vite только при старте dev-сервера / сборке. Изменил значение — перезапусти `npm run dev` или контейнер.
- Бэкенд вне Development-окружения **падает при старте**, если ключ не задан или равен dev-ключу — это сделано специально, чтобы прод не поднялся с пустым/дефолтным ключом.

Проверить ключ руками:

```bash
# без ключа — 401
curl -X DELETE http://localhost:5000/api/strats/1

# с ключом — ок
curl -H "X-Api-Key: mapify-dev-admin-key" http://localhost:5000/api/submissions/admin/strats
```

## Структура репозитория

```text
Back/MapifyBackend/       # ASP.NET Core API (+ интеграционные тесты)
Front/Admin-panel/        # админ-панель (рабочее приложение — в подпапке admin-panel/)
Front/Discord/            # Discord-бот (сервис discord-bot в docker-compose, нужен Front/Discord/.env)
Front/Telegram/           # Telegram-бот (сервис telegram-bot в docker-compose, нужен Front/Telegram/.env)
docker-compose.yaml       # backend + frontend + telegram-bot + discord-bot (+ профиль test)
```

Подробности по каждой части — в `AGENTS.md` соответствующих папок и в `Back/MapifyBackend/API_DOCUMENTATION.md`.
