# Mapify Admin Panel — инструкция для AI-агентов

> Этот файл описывает фронтенд-часть Mapify Admin Panel. Проект — админ-панель для модерации игровых стратегий Rainbow Six Siege.
> Ориентируйся на фактическое содержимое репозитория; не делай предположений о стеке, которого нет в коде.

## Обзор проекта

Mapify Admin Panel — одностраничное веб-приложение (SPA), написанное на Vue 3. Оно предоставляет UI для:

- просмотра дашборда с pending-заявками и общей статистикой;
- модерации заявок на стратки и категории (approve / reject / просмотр);
- управления стратками (CRUD, привязка категорий и оперативников);
- управления категориями (create/delete, редактирование пока не поддерживается бэкендом);
- просмотра оперативников и карт (placeholder-страницы).

Бэкенд — отдельный ASP.NET Core проект, расположенный в `C:/prog/Mapify/Back/MapifyBackend`. Фронтенд общается с ним по REST API.

## Технологический стек

- **Фреймворк:** Vue 3 (`^3.5.40`), Composition API, `<script setup>`.
- **Сборщик:** Vite 8 (`^8.2.0`) с плагином `@vitejs/plugin-vue`.
- **Стили:** Tailwind CSS v4 (`^4.3.3`) через `@tailwindcss/vite`.
- **Роутинг:** Vue Router 4 (`^5.2.0`), режим `createWebHistory`.
- **HTTP-клиент:** собственная обёртка над `fetch` (`src/api/client.js`), никаких axios/Apollo нет.
- **Уведомления:** собственный composable `useToast` (`src/composables/useToast.js`).
- **Глобальное состояние:** в проекте **нет Pinia/Vuex**; состояние держится в компонентах через `ref`/`computed`.

В корне репозитория (`C:/prog/Mapify/Front/Admin-panel/package.json`) есть отдельный `package.json` с зависимостями `@element-plus/icons-vue`, `element-plus`, `pinia`, `vue-router`. Он **не используется** для сборки приложения. Рабочее приложение находится в подкаталоге `admin-panel/`.

## Структура репозитория

```text
C:/prog/Mapify/Front/Admin-panel
├── package.json              # устаревший/неиспользуемый манифест (Element Plus, Pinia)
├── package-lock.json         # lock-файл для корневого package.json
├── plan.md                   # план/статус работы над проектом
└── admin-panel/              # рабочее Vue 3 приложение
    ├── package.json          # манифест приложения
    ├── vite.config.js        # конфиг Vite
    ├── index.html            # точка входа
    ├── README.md             # стандартный README от шаблона Vue 3 + Vite
    ├── SESSION_STATE.md      # заметки по состоянию сессии (ручные команды, известные ограничения)
    ├── FIGMA_PROMPT.md       # промпт для генерации дизайна в Figma
    ├── dist/                 # результат production-сборки
    ├── public/               # статические assets
    └── src/
        ├── main.js           # создание приложения, подключение роутера и стилей
        ├── App.vue           # корневой layout: Sidebar + Header + router-view + ToastContainer
        ├── style.css         # Tailwind-импорт, кастомная тема, анимации
        ├── router/index.js   # объявление маршрутов
        ├── api/              # API-клиент и модули по сущностям
        │   ├── client.js     # fetch-обёртка
        │   ├── strats.js
        │   ├── categories.js
        │   ├── operators.js
        │   ├── maps.js
        │   └── submissions.js
        ├── views/            # страницы, привязанные к маршрутам
        │   ├── DashboardView.vue
        │   ├── PendingSubmissionsView.vue
        │   ├── StratsView.vue
        │   ├── StratDetailView.vue
        │   ├── CategoriesView.vue
        │   ├── OperatorsView.vue
        │   └── MapsView.vue
        ├── components/       # переиспользуемые компоненты
        │   ├── Sidebar.vue
        │   ├── Header.vue
        │   ├── ToastContainer.vue
        │   ├── SideBadge.vue
        │   ├── StatusBadge.vue
        │   ├── StratFormModal.vue
        │   ├── CategoryFormModal.vue
        │   ├── SubmissionDetailSlideOver.vue
        │   ├── VideoPlayer.vue
        │   ├── PlaceholderPage.vue
        │   ├── SubmissionDetail.vue
        │   ├── SubmissionList.vue
        │   └── HelloWorld.vue
        ├── composables/
        │   └── useToast.js
        └── assets/           # изображения и иконки
```

## Команды сборки и запуска

Все команды выполняются из директории `admin-panel/`:

```bash
# Установка зависимостей
npm install

# Dev-сервер (по умолчанию http://localhost:5173)
npm run dev

# Production-сборка; результат попадает в admin-panel/dist/
npm run build

# Локальный просмотр production-сборки
npm run preview
```

Проверка перед коммитом — минимум `npm run build` без ошибок.

### Ручной запуск в связке с бэкендом

Если нужно тестировать функционал end-to-end:

```bash
# Бэкенд (в отдельном терминале)
cd /c/prog/Mapify/Back/MapifyBackend
dotnet run --project MapifyBackend/MapifyBackend.csproj --urls "http://localhost:5000"

# Фронтенд
cd /c/prog/Mapify/Front/Admin-panel/admin-panel
npm run dev
```

Фронтенд по умолчанию ходит на `http://localhost:5000/`. Базовый URL можно переопределить через переменную окружения `VITE_API_BASE_URL`.

## Архитектура приложения

### Роутинг

Файл: `admin-panel/src/router/index.js`.

| Путь | Компонент | Назначение |
|------|-----------|------------|
| `/` | redirect → `/dashboard` | |
| `/dashboard` | `DashboardView` | сводка статистики и recent activity |
| `/pending` | `PendingSubmissionsView` | модерация заявок на стратки/категории |
| `/strats` | `StratsView` | список страток, создание/редактирование/удаление |
| `/strats/:id` | `StratDetailView` | детальная карточка стратки |
| `/categories` | `CategoriesView` | управление категориями |
| `/operators` | `OperatorsView` | placeholder |
| `/maps` | `MapsView` | placeholder |

### API-клиент

Файл: `admin-panel/src/api/client.js`.

- Базовый URL: `import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/'`.
- Экспортируются функции `get`, `post`, `put`, `patch`, `del`.
- Каждая функция:
  - собирает полный URL с trailing slash у базы;
  - добавляет заголовки `Accept: application/json` и, для mutating-запросов, `Content-Type: application/json`;
  - парсит JSON;
  - при `!response.ok` бросает `Error` с текстом из `data.error || data.message || HTTP ${status}`;
  - возвращает `data.data ?? data`.

API-модули (`strats.js`, `categories.js`, `operators.js`, `maps.js`, `submissions.js`) — тонкие обёртки вокруг `client.js`, сгруппированные по предметной области. Добавляй новые вызовы именно туда, а не прямо в компоненты.

### Состояние и уведомления

- `useToast` — глобальный singleton-массив `toasts`, методы `addToast(message, type, duration)` и `removeToast(id)`. Типы: `success`, `error`, `info`.
- В `App.vue` загружаются pending-заявки, формируются уведомления и передаются в `Header`, а счётчик — в `Sidebar`.
- Компоненты views сами загружают свои данные в `onMounted` и хранят их в локальных `ref`.

### Видеоплеер

Компонент `VideoPlayer.vue` определяет платформу по URL и рендерит:

- YouTube — `iframe` с `youtube.com/embed/{id}`;
- TikTok — `iframe` с `tiktok.com/embed/v2/{id}`;
- Instagram — `iframe` с `instagram.com/p/{id}/embed`;
- прямые ссылки на `.mp4/.webm/.ogg/.mov` — нативный `<video>`;
- нераспознанные URL — заглушка со ссылкой.

Используется в `StratDetailView` и `SubmissionDetailSlideOver`.

## Стилевые соглашения

- Тёмная тема, цвета заданы в `admin-panel/src/style.css` через `@theme`:
  - фон `#0B0B0C`, поверхности `#141416`/`#1C1C1F`;
  - акцент/атака `#DC2626`, hover `#EF4444`;
  - защита `#2563EB`;
  - успех `#16A34A`.
- Шрифты: Inter (основной), JetBrains Mono (моноширинный), Rajdhani (заголовки/подписи).
- Классы Tailwind используются инлайново, в том числе с произвольными значениями (`bg-[#0B0B0C]`, `w-[240px]`).
- В `style.css` также определены собственные keyframes и утилиты: `animate-fade-in`, `animate-slide-right`, `animate-toast`, `glow-red`, `table-row-hover`, `skeleton`.
- Иконки — inline SVG из Heroicons ( stroke `currentColor`, `viewBox="0 0 24 24"`).
- Бейджи сторон: `SideBadge` (Attack/Defense); статусов: `StatusBadge` (pending/approved/rejected).

## Правила при работе с кодом

1. **Не используй Pinia/Vuex** без явной договорённости. Текущая архитектура опирается на локальное состояние компонентов и пропсы.
2. **Все HTTP-вызовы** должны идти через `src/api/*.js`, а не напрямую из компонентов.
3. **Новые маршруты** добавляй в `src/router/index.js` и создавай view в `src/views/`.
4. **Уведомления об успехе/ошибке** выводи через `useToast()`.
5. **Стили** добавляй через Tailwind-утилиты; если нужна глобальная тема/анимация — в `src/style.css`.
6. **Компоненты** пиши как SFC с `<script setup>`.
7. **Перед коммитом** запускай `npm run build` в `admin-panel/` и убеждайся, что сборка проходит без ошибок.
8. **Не правь файлы в корне** (`package.json`, `app.js`) для нужд приложения — они не участвуют в сборке.

## Тестирование

- В проекте **не установлен фреймворк для тестирования** и нет автотестов.
- Основная проверка качества — `npm run build`.
- Ручное тестирование требует запущенного бэкенда на `http://localhost:5000`.
- Если понадобится добавить юнит-тесты, логичный выбор для Vite/Vue — `vitest` + `@vue/test-utils`. Добавляй их как `devDependencies` в `admin-panel/package.json`.

## Безопасность

- Приложение не реализует аутентификацию/авторизацию на клиенте. Любой, у кого есть доступ к dev-серверу, может выполнять запросы к бэкенду.
- Базовый URL бэкенда вынесен в `VITE_API_BASE_URL`; не хардкоди продакшен-URL в коде.
- `VideoPlayer` встраивает iframe с внешних доменов (YouTube, TikTok, Instagram). При жёстком CSP или production-развёртывании учитывай необходимость разрешить эти источники.
- Никаких секретов (ключи API, пароли) в репозитории нет; не добавляй `.env` с секретами в коммит.
- `client.js` не добавляет токены авторизации в заголовки. Если бэкенд начнёт требовать авторизацию, дорабатывай `client.js` централизованно.

## Известные ограничения

- Бэкенд не поддерживает редактирование категорий — кнопка Edit в `CategoriesView` показывает ошибку через toast.
- У категорий в бэкенде нет полей `Description` и `CreatedAt`, поэтому в таблице отображаются только `ID`, `Name`, `Side`, `Actions`.
- Страницы `/operators` и `/maps` — placeholder'ы (`PlaceholderPage`).
- `HelloWorld.vue`, `SubmissionDetail.vue`, `SubmissionList.vue` и `AdminView.vue` в текущей реализации не используются роутером (последний пустой), но остаются в кодовой базе.

## Полезные ссылки

- `admin-panel/SESSION_STATE.md` — актуальное состояние фронтенда/бэкенда и команды для ручного запуска.
- `admin-panel/FIGMA_PROMPT.md` — дизайн-промпт и цветовая палитра.
- Бэкенд и его API описаны в `Back/MapifyBackend/API_DOCUMENTATION.md` и `Back/MapifyBackend/AGENTS.md`.
