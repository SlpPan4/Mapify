# План / статус Mapify Admin Panel

## Контекст
- Фронтенд: `C:/prog/Mapify/Front/Admin-panel/admin-panel` (Vue 3 + Tailwind + Vue Router).
- Бэкенд: `C:/prog/Mapify/Back/MapifyBackend/MapifyBackend` (ASP.NET Core 10 + SQLite + Dapper).

## Что сделано на текущем шаге (backend)
- [x] Создан `MapService` (`MapifyBackend/database_files/Services/MapService.cs`).
- [x] В `StratService` добавлен метод `RemoveCategoryFromStrat`.
- [x] `MapsController` переписан на использование `MapService` вместо прямого вызова `_db`.
- [x] `StratsController` больше не зависит от `DatabaseService` напрямую:
  - `GET /api/strats/maps/{id}` → `MapService`;
  - `DELETE /api/strats/{stratId}/categories/{categoryId}` → `StratService`.
- [x] `MapService` зарегистрирован как scoped в `Program.cs`.
- [x] Обновлена документация:
  - `API_DOCUMENTATION.md` — добавлены `/api/maps`, `/api/strats/summary`, `DELETE /api/strats/{id}/categories/{catId}`, поле `Description` в `POST /api/strats`.
  - `AGENTS.md` — добавлены `MapsController`, `MapService`, `/api/maps` в структуру и роутинг.
- [x] Добавлены интеграционные тесты:
  - `MapsControllerTests` — `GET /api/maps`;
  - `StratsControllerTests` — `GET /api/strats/summary`, `DELETE /api/strats/{id}/categories/{catId}`, проверка сохранения `Description` при создании стратки.
- [x] Бэкенд собран и тесты пройдены: `dotnet test MapifyBackend.sln` → 72 passed.

## Объяснение решения по сервисам
`MapsController` изначально обращался к `_db` напрямую, что нарушало единый сервисный слой проекта. `OperatorsController` уже работал через `OperatorService`, поэтому его трогать не потребовалось. Я привёл `MapsController` и новый функционал `StratsController` к тому же паттерну: контроллеры вызывают только сервисы, а SQL/Dapper остаётся в `DatabaseService`. Это упрощает тестирование и держит архитектуру консистентной.

## Осталось
- [x] Пересобрать фронтенд (`npm run build` в `admin-panel`) — сборка прошла успешно.
- [ ] (По желанию) Добавить видеопроигрыватель в просмотре страток и поддержку импорта видео из TikTok/Instagram.

## Ключевые файлы
- `Back/MapifyBackend/MapifyBackend/database_files/Services/MapService.cs`
- `Back/MapifyBackend/MapifyBackend/database_files/Services/StratService.cs`
- `Back/MapifyBackend/MapifyBackend/Controllers/MapsController.cs`
- `Back/MapifyBackend/MapifyBackend/Controllers/StratsController.cs`
- `Back/MapifyBackend/MapifyBackend/Program.cs`
- `Back/MapifyBackend/API_DOCUMENTATION.md`
- `Back/MapifyBackend/AGENTS.md`
- `Back/MapifyBackend/MapifyBackend.IntegrationTests/MapsControllerTests.cs`
- `Back/MapifyBackend/MapifyBackend.IntegrationTests/StratsControllerTests.cs`
