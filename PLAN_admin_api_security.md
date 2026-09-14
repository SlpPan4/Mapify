# План: защита админ-API + чистка админ-панели

> **Как продолжить в новой сессии:** скажи агенту — «продолжи работу по `PLAN_admin_api_security.md`».
> Агент читает этот файл, смотрит невыполненные чекбоксы и `git status`, и продолжает с того места.
> Состояние на момент создания: ветка `feature/admin-api-security` создана, рабочее дерево чистое, код ещё не тронут.

## Контекст и ключевые решения (уже согласованы)

- **Ветка:** `feature/admin-api-security` (создана). **Коммит делает пользователь сам** — агент не коммитит.
- **Dev-режим:** публичный ключ `mapify-dev-admin-key`, лежит в `appsettings.Development.json`, коммитить безопасно, можно давать друзьям.
- **Prod-режим:** ключ только из переменной окружения (`AdminApi__Key`); если окружение не Development и ключ пустой или равен dev-ключу — приложение падает при старте с понятной ошибкой.
- **Заголовок:** `X-Api-Key`.
- **Защищаем:** все `/api/submissions/admin/...` + mutating-запросы (POST/PUT/DELETE/PATCH) к `/api/strats`, `/api/categories`, `/api/operators`.
- **Не защищаем:** все GET + `POST /api/submissions/strats` и `POST /api/submissions/categories` (публичные заявки).
- **401** при отсутствии/неверном ключе, JSON `{ error = "..." }` в стиле существующих контроллеров.
- **OPTIONS (CORS preflight)** middleware пропускает без ключа.

## Этап 1. Бекенд (`Back/MapifyBackend`)

Перед работой читать `Back/MapifyBackend/AGENTS.md` (архитектура, конвенции, команды).

- [x] **1.1. API-ключ middleware** — новый файл `MapifyBackend/Utility/Api/ApiKeyAuthMiddleware.cs`, регистрация в `Program.cs`. Правила защиты — см. «Ключевые решения» выше. Ключ из конфига `AdminApi:Key`; dev-fallback — `mapify-dev-admin-key`; prod fail-fast при пустом/dev-ключе. (Ключ читается после `builder.Build()`, иначе тестовые override'ы конфигурации не успевают примениться.)
- [x] **1.2. `appsettings.Development.json`** — добавить `"AdminApi": { "Key": "mapify-dev-admin-key" }` и `"Cors": { "AllowedOrigins": ["http://localhost:5173"] }`.
- [x] **1.3. CORS** — named policy `AppCors` вместо AllowAll: origins из `Cors:AllowedOrigins`; пустой список в Development → AllowAll (удобство), в Production → deny by default. Методы/заголовки — AllowAnyMethod/AllowAnyHeader внутри разрешённых origins.
- [ ] **1.4. SQLite-пакеты** — поднять `Microsoft.Data.Sqlite` (и при необходимости EF Core SQLite) до актуальной версии под net10.0, чтобы ушёл advisory GHSA-2m69-gcr7-jv3q (`SQLitePCLRaw.lib.e_sqlite3` 2.1.11). Проверка: `dotnet list package --vulnerable`.
- [x] **1.5. Тесты** — новый `MapifyBackend.IntegrationTests/AdminAuthTests.cs`:
  - без ключа на admin-эндпоинт → 401; с неверным ключом → 401; с верным → успех;
  - mutating без ключа → 401;
  - публичный GET (`/api/maps`) без ключа → 200;
  - `POST /api/submissions/...` без ключа → не 401.
  - `CustomWebApplicationFactory` задаёт тестовый ключ (например `test-admin-key`); `ControllerTestsBase` добавляет `X-Api-Key: test-admin-key` в HttpClient по умолчанию, чтобы существующие тесты не ломались; AdminAuthTests использует клиент без заголовка.
- [x] **1.6. Прогон** — `dotnet build MapifyBackend.sln` и `dotnet test MapifyBackend.sln` зелёные (78/78).
- [x] **1.7. Доки бекенда** — обновить `API_DOCUMENTATION.md` (раздел про `X-Api-Key`, список защищённых эндпоинтов) и `AGENTS.md` (переписать пометки "No authentication", "CORS is wide open"; описать dev/prod ключи).

## Этап 2. Админ-панель (`Front/Admin-panel/admin-panel`)

Перед работой читать `Front/Admin-panel/AGENTS.md` (стек Vue 3 + Vite + Tailwind v4, правила: без Pinia, HTTP только через `src/api/*.js`).

- [x] **2.1. Ключ в `src/api/client.js`** — заголовок `X-Api-Key` на все запросы; ключ из `import.meta.env.VITE_API_KEY`, fallback `'mapify-dev-admin-key'`; при 401 — Error с понятным сообщением (покажется через useToast).
- [x] **2.2. `.env.example`** в `admin-panel/` с `VITE_API_KEY=mapify-dev-admin-key` и комментарием про prod. Проверить, что `.env` в .gitignore.
- [x] **2.3. Чистка мёртвого кода** (перед удалением проверить Grep'ом отсутствие импортов):
  - корень `Front/Admin-panel/`: `package.json`, `package-lock.json`, `app.js` (неиспользуемый манифест с Element Plus/Pinia);
  - `admin-panel/src/components/`: `HelloWorld.vue`, `SubmissionDetail.vue`, `SubmissionList.vue`;
  - `admin-panel/src/views/AdminView.vue` (пустой, роутером не используется);
  - `admin-panel/dist/` — добавить в .gitignore и удалить из рабочего дерева (если отслеживается гитом — просто удалить файлы, deletion закоммитит пользователь).
- [ ] **2.4. Read-only страницы** `/operators` и `/maps` — вместо PlaceholderPage простые таблицы в стиле `CategoriesView.vue` (тёмная тема, Tailwind, useToast, загрузка в onMounted, SideBadge для стороны оперативника). Данные: GET `/api/operators`, GET `/api/maps`. Без CRUD.
- [x] **2.5. Прогон** — `npm run build` из `admin-panel/` без ошибок.
- [x] **2.6. Доки фронта** — обновить `Front/Admin-panel/AGENTS.md`: секции «Безопасность» (X-Api-Key, VITE_API_KEY), «Известные ограничения», структура репозитория.

## Этап 3. Финальная проверка

- [x] **3.1.** `dotnet test MapifyBackend.sln` — все тесты зелёные (78/78).
- [x] **3.2.** `npm run build` — без ошибок.
- [x] **3.3.** Smoke-проверка выполнена в Docker: `docker compose up -d --wait`, проверено — публичный GET 200, mutating/admin без ключа 401, с dev-ключом 200, админка на 5173 отвечает.
- [ ] **3.4.** Отчёт по diff'у пользователю. **Коммит — пользователь.**

## Осознанно НЕ входит в этот план

- JWT / пользователи / роли — оверкилл на этом этапе.
- Редактирование категорий в бекенде (кнопка Edit в админке) — отдельная задача.
- Ротация/шифрование prod-ключей — когда появится реальный прод.

## Заметки по выполнению

- Этапы 1 и 2 независимы — можно делать параллельно двумя сабагентами.
- Стоп-слово для проверок: перед завершением обязательны зелёные `dotnet test` и `npm run build`.
- После каждого выполненного пункта отмечать чекбокс в этом файле (`[x]`), чтобы следующая сессия видела прогресс.
- Осталось вне выполненного: **1.4** (обновление SQLite-пакетов против GHSA-2m69-gcr7-jv3q) и **2.4** (read-only страницы `/operators` и `/maps` — сейчас там всё ещё `PlaceholderPage`).
- Дополнительно сделано: корневой `README.md` (мануал по запуску), корневой `.env.example`, `docker-compose.yaml` пересобран под одну команду (Discord-бот из compose убран по решению пользователя), базовые образы бэкенда переведены с `10.0-preview` на `10.0`.
- Внимание: Discord-бот (`Front/Discord/cmds/utils/stratadd.js`, `stratdel.js`) ходит на mutating-эндпоинты **без** `X-Api-Key` — после включения middleware он получает 401. Доработка бота — за его ответственным.
