# Mapify — мануал по запуску

Стек: бэкенд на ASP.NET Core (.NET 10) + админ-панель на Vue 3 (Vite). Данные — в SQLite-файле.

## Быстрый старт: Docker (одна команда)

Нужен только установленный Docker Desktop. Из корня репозитория:

```bash
docker compose up --build
```

Поднимутся два сервиса:

- **backend** — API на http://localhost:5000
- **frontend** — админ-панель на http://localhost:5173

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

`docker compose up` работает без всякой настройки: по умолчанию используется dev-ключ `mapify-dev-admin-key`.

Чтобы задать свой ключ, создай в **корне репозитория** файл `.env` (рядом с `docker-compose.yaml`, см. `.env.example`):

```
ADMIN_API_KEY=мой-секретный-ключ
```

Compose сам подхватит его и передаст и в бэкенд (`AdminApi__Key`), и в админку (`VITE_API_KEY`). После изменения: `docker compose up --build` (или хотя бы `docker compose restart`).

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
Front/Discord/            # Discord-бот (в docker-compose не входит, настраивается отдельно)
Front/Telegram/           # Telegram-бот (в docker-compose не входит)
docker-compose.yaml       # backend + frontend (+ профиль test)
```

Подробности по каждой части — в `AGENTS.md` соответствующих папок и в `Back/MapifyBackend/API_DOCUMENTATION.md`.
