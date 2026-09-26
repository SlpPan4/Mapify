# Mapify Discord Bot

Discord-бот Mapify: выдаёт стратегии Rainbow Six Siege из бэкенда, принимает новые стратегии на модерацию и показывает статистику игроков с R6 Tracker (tracker.gg).

## Стек

- Node.js 22+ (ESM)
- discord.js v14 (slash-команды)
- HTTP — встроенный `fetch`, без сторонних клиентов

## Команды

| Команда | Описание |
|---|---|
| `/strats <id>` | Показать стратегию по ID |
| `/strats-list` | Все стратегии с пагинацией кнопками |
| `/strat-submit` | Отправить стратегию на модерацию (см. ниже) |
| `/r6stats <platform> <nickname> [section]` | Статистика игрока R6 Siege с tracker.gg (профиль / плейлисты / оперативники) |
| `/ping` | Шуточный «пинг до серверов R6» |
| `/help` | Список команд |

## Модерация: только публичные API

Бот **намеренно не имеет ключа админки** (`X-Api-Key`) и работает с бэкендом только через публичные эндпоинты:

- чтение: `GET /api/strats`, `GET /api/strats/{id}`, `GET /api/strats/maps/{id}`, `GET /api/maps`;
- подача на модерацию: `POST /api/submissions/strats`.

Поток: пользователь вызывает `/strat-submit` → заявка попадает в pending-таблицу → админ одобряет её в админ-панели (`/api/submissions/admin/...`) → стратегия становится видна в `/strats` и `/strats-list`. Прямого добавления или удаления стратегий из бота нет.

## Настройка

Скопируй `.env.example` в `.env` и заполни:

| Переменная | Обязательна | Назначение |
|---|---|---|
| `DISCORD_TOKEN` | да | токен бота (Developer Portal → Bot) |
| `DISCORD_CLIENT_ID` | да | Application ID (Developer Portal → General Information) |
| `API_BASE_URL` | нет | базовый URL бэкенда (по умолчанию `http://localhost:5000`; в docker-compose подставляется `http://backend:5000`) |
| `TRN_API_KEY` | для `/r6stats` | ключ Tracker Network API (https://tracker.gg/developers) |
| `LOG_CHANNEL_ID` | нет | канал для логов входа/выхода бота с серверов |
| `LOG_PING_USER_ID` | нет | кого пинговать в лог-канале |

## Запуск

Локально:

```bash
npm install
npm start        # или npm run dev (nodemon)
```

В Docker — бот входит в корневой `docker-compose.yaml` (сервис `discord-bot`):

```bash
docker compose up --build discord-bot
```

Compose читает `Front/Discord/.env` (см. выше) и переопределяет `API_BASE_URL` на `http://backend:5000`. Без `.env` контейнер завершится с ошибкой про `DISCORD_TOKEN`/`DISCORD_CLIENT_ID` — на остальные сервисы это не влияет.

Slash-команды регистрируются глобально при каждом старте бота (полная замена списка).

## Структура

```text
Front/Discord/
├── index.js            # точка входа: загрузка команд/событий, регистрация slash-команд, login
├── api/
│   ├── api.js          # клиент бэкенда Mapify (только публичные эндпоинты)
│   └── r6tracker.js    # клиент Tracker Network API (кеш 5 мин, обработка 404/429/таймаутов)
├── cmds/utils/         # slash-команды (по файлу на команду)
├── Events/             # обработчики событий Discord
└── DOCKERFILE
```
