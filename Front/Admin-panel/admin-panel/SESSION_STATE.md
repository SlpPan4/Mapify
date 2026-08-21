# Состояние сессии — переписывание админ-панели Mapify на Vue 3

> Этот файл создан для восстановления работы в новой сессии. Если сессия прервалась, начни отсюда.

## Статус: основная реализация завершена + VideoPlayer + фиксы

Frontend и backend успешно собираются. Серверы могут быть запущены вручную (см. команды ниже).

## Что сделано

### Backend (MapifyBackend)
- `DatabaseService.GetAllMaps()` + `MapsController` → `GET /api/maps`
- `DatabaseService.RemoveCategoryFromStrat()` + `StratsController` → `DELETE /api/strats/{id}/categories/{catId}`
- `DatabaseService.GetStratsSummary()` + `StratsController` → `GET /api/strats/summary`
- DTO `StratSummary.cs`
- **FIX:** `StratRequest` теперь принимает `Description`, и `StratService.CreateStrat` сохраняет её в БД.

### Frontend (admin-panel)
- Зависимости: Tailwind CSS v4, Vue Router.
- Роутинг: `/dashboard`, `/pending`, `/strats`, `/strats/:id`, `/categories`, `/operators`, `/maps`.
- API-клиент и модули для всех сущностей.
- Layout: Sidebar, Header, Toast.
- Views: Dashboard, Pending Submissions, Strats, Strat Detail, Categories, Operators (placeholder), Maps (placeholder).
- Компоненты: SideBadge, StatusBadge, SubmissionDetailSlideOver, StratFormModal, CategoryFormModal, PlaceholderPage, VideoPlayer.
- **FIX:** Уведомления в Header теперь реальные — формируются из pending strat/category submissions с относительным временем. Красная точка показывается только при наличии уведомлений.
- **VideoPlayer** — универсальный компонент для проигрывания видео из YouTube, TikTok, Instagram и прямых ссылок (mp4/webm/ogg/mov). Используется в Strat Detail и Submission Detail Slide-over.

### Frontend (admin-panel)
- Зависимости: Tailwind CSS v4, Vue Router.
- Роутинг: `/dashboard`, `/pending`, `/strats`, `/strats/:id`, `/categories`, `/operators`, `/maps`.
- API-клиент и модули для всех сущностей.
- Layout: Sidebar, Header, Toast.
- Views: Dashboard, Pending Submissions, Strats, Strat Detail, Categories, Operators (placeholder), Maps (placeholder).
- Компоненты: SideBadge, StatusBadge, SubmissionDetailSlideOver, StratFormModal, CategoryFormModal, PlaceholderPage, VideoPlayer.
- **VideoPlayer** — универсальный компонент для проигрывания видео из YouTube, TikTok, Instagram и прямых ссылок (mp4/webm/ogg/mov). Используется в Strat Detail и Submission Detail Slide-over.

## Проверка

- `npm run build` — успешно.
- `dotnet build MapifyBackend/MapifyBackend.csproj` — успешно (warnings, 0 errors).
- Backend отвечает на `GET /api/strats`, `/api/maps`, `/api/strats/summary`.
- `POST /api/strats` создаёт страт.

## Известные ограничения

- Backend не поддерживает редактирование категорий (только create/delete). В UI edit category показывает toast об ошибке.
- Категории в backend не имеют Description и CreatedAt, поэтому в таблице категорий отображаются только ID, Name, Side, Actions.
- Operators / Maps pages — placeholder, так как backend endpoints для управления ими есть, но UX для полноценного управления не реализован.

## Если нужно продолжить

1. Открыть `http://localhost:5173` в браузере.
2. Проверить функционал: approve/reject submissions, create/edit/delete strats, create/delete categories.
3. Исправить выявленные runtime-ошибки.
4. По желанию: добавить редактирование категорий в backend, улучшить loading/empty states, добавить адаптивность.

## Команды для ручного запуска

```bash
# Backend (если не запущен)
cd /c/prog/Mapify/Back/MapifyBackend
dotnet run --project MapifyBackend/MapifyBackend.csproj --urls "http://localhost:5000"

# Frontend (если не запущен)
cd /c/prog/Mapify/Front/Admin-panel/admin-panel
npm run dev
```
