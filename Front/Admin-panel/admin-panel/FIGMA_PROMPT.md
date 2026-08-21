# Промпт для Figma: дизайн админ-панели Mapify

Этот файл содержит готовый промпт для генерации дизайна админ-панели в Figma (Figma AI / Make Design). Проект: **Mapify** — админка для модерации игровых стратегий Rainbow Six Siege.

> **Совет:** Figma AI лучше понимает английский язык. Используйте английскую версию промпта ниже для копирования в Figma. Русская версия дана для справки и ручной доработки.

---

## 🇬🇧 English prompt (copy this into Figma AI)

```text
Design a dark-themed admin dashboard for "Mapify" — a content moderation panel for a Rainbow Six Siege strategy database.

Color palette (strict black-and-red theme):
- Background primary: #0B0B0C
- Background secondary/surface: #141416
- Background elevated (cards, modals): #1C1C1F
- Border/divider: #2A2A2E
- Text primary: #FFFFFF
- Text secondary/muted: #9CA3AF
- Accent red: #DC2626
- Accent red hover: #EF4444
- Accent red glow: rgba(220, 38, 38, 0.35)
- Success/approve: #16A34A (green)
- Reject/danger: #DC2626 (red)
- Attack side badge: #DC2626
- Defense side badge: #2563EB

Typography:
- Font: Inter, Roboto, or SF Pro Display
- Headings: SemiBold / Bold
- Body: Regular / Medium
- Small labels/captions: 12-13px, muted color

Layout structure:
1. Left sidebar (fixed, 240px wide):
   - Mapify logo at top
   - Navigation items: Dashboard, Pending Submissions, Strats, Categories, Operators, Maps
   - Red notification badges showing pending counts
   - Admin profile / logout at bottom

2. Top header bar:
   - Global search for strats/categories/submissions
   - Refresh button
   - Notification bell with red dot
   - Admin avatar

3. Dashboard overview page:
   - 4 statistic cards in a row: Pending Strats, Pending Categories, Total Strats, Total Categories
   - Red accent numbers and subtle glow on hover
   - Recent activity list/table showing latest submissions with status
   - Quick-action buttons: "Approve Selected", "Reject Selected"

4. Pending Submissions page:
   - Tabs: "Strat Submissions" | "Category Submissions"
   - Data table columns: Checkbox, ID, Name, Side, Map, Submitted At, Status, Actions
   - Row actions: View (eye icon), Approve (checkmark), Reject (X icon)
   - Filters: by Side (Attack/Defense), by Date, by Search
   - Bulk selection with top toolbar
   - Empty state illustration/message

5. Submission detail modal/side panel:
   - Submission title and ID
   - Side badge (Attack/Defense)
   - Video preview / video URL field
   - Description text block
   - Map name
   - Category chips and Operator chips
   - Submitted date
   - Primary CTA: "Approve" (red filled button)
   - Secondary CTA: "Reject" (dark button with red border)
   - Tertiary: "Edit before approve"

6. Strats / Categories management page:
   - Search + filter bar
   - Data table with columns: ID, Name, Side, Map (strats only), Actions
   - Buttons: "Add New", "Edit", "Delete" (delete in red)
   - Modal form for create/edit

UX requirements:
- Minimalist, clean, high contrast
- Clear visual hierarchy with generous spacing
- Table rows highlight on hover with subtle red tint
- Buttons have hover states and active states
- Loading skeletons and empty states
- Toast notifications for approve/reject success
- Dark mode only, no light theme
- Tactical, modern gaming aesthetic inspired by Rainbow Six Siege UI
- Desktop-first layout, but keep tablet adaptability in mind

Deliverables:
- Full desktop dashboard screen
- Pending submissions list screen
- Submission detail modal / slide-over
- Create/Edit modal for strats and categories
- Components: buttons, badges, inputs, table, cards, sidebar nav
```

---

## 🇷🇺 Русская версия (для справки)

```text
Создай дизайн тёмной админ-панели для приложения Mapify — панели модерации контента базы данных стратегий Rainbow Six Siege.

Цветовая палитра (строгая чёрно-красная тема):
- Основной фон: #0B0B0C
- Вторичный фон / поверхности: #141416
- Приподнятые поверхности (карточки, модалки): #1C1C1F
- Границы / разделители: #2A2A2E
- Основной текст: #FFFFFF
- Вторичный текст: #9CA3AF
- Акцентный красный: #DC2626
- Красный при наведении: #EF4444
- Свечение красного: rgba(220, 38, 38, 0.35)
- Успех / принять: #16A34A
- Опасность / отклонить: #DC2626
- Бейдж Attack: #DC2626
- Бейдж Defense: #2563EB

Типографика:
- Шрифт: Inter, Roboto или SF Pro Display
- Заголовки: SemiBold / Bold
- Основной текст: Regular / Medium
- Подписи: 12–13px, приглушённый цвет

Структура интерфейса:
1. Боковое меню слева (фиксированное, 240px):
   - Логотип Mapify вверху
   - Пункты: Dashboard, Pending Submissions, Strats, Categories, Operators, Maps
   - Красные бейджи с количеством ожидающих заявок
   - Профиль админа / выход внизу

2. Верхняя панель:
   - Глобальный поиск по стратам/категориям/заявкам
   - Кнопка обновления
   - Колокольчик уведомлений с красной точкой
   - Аватар админа

3. Главная страница Dashboard:
   - 4 карточки статистики в ряд: Pending Strats, Pending Categories, Total Strats, Total Categories
   - Красные акцентные цифры и лёгкое свечение при наведении
   - Список/таблица последней активности
   - Быстрые действия: "Approve Selected", "Reject Selected"

4. Страница Pending Submissions:
   - Вкладки: Strat Submissions | Category Submissions
   - Таблица с колонками: Checkbox, ID, Name, Side, Map, Submitted At, Status, Actions
   - Действия в строке: View, Approve, Reject
   - Фильтры: по Side, дате, поиску
   - Массовый выбор с верхней панелью действий
   - Empty state

5. Детальная карточка заявки (модалка / выдвижная панель):
   - Название и ID заявки
   - Бейдж Side
   - Превью видео / поле videoUrl
   - Блок описания
   - Название карты
   - Чипсы категорий и оперативников
   - Дата подачи
   - Кнопки: Approve (красная), Reject (тёмная с красной рамкой), Edit before approve

6. Страница управления Strats / Categories:
   - Поиск + фильтры
   - Таблица с ID, Name, Side, Map (только для страт), Actions
   - Кнопки: Add New, Edit, Delete
   - Модальное окно создания/редактирования

Требования к UX:
- Минимализм, чистота, высокая контрастность
- Чёткая визуальная иерархия и большие отступы
- Строки таблиц подсвечиваются при наведении с лёгким красным оттенком
- У кнопок есть состояния hover и active
- Состояния загрузки и пустого списка
- Toast-уведомления об успешном approve/reject
- Только тёмная тема
- Тактический современный игровой стиль в духе UI Rainbow Six Siege
- Desktop-first, но с возможностью адаптации под планшет

Что нужно отрисовать:
- Полноэкранный Dashboard
- Список Pending Submissions
- Детальная карточка заявки
- Модалка создания/редактирования
- Компоненты: кнопки, бейджи, инпуты, таблица, карточки, навигация
```

---

## 🎨 Быстрая цветовая палитра для Figma

| Токен | Значение | Использование |
|-------|----------|---------------|
| `bg-primary` | `#0B0B0C` | Основной фон |
| `bg-surface` | `#141416` | Панели, sidebar |
| `bg-elevated` | `#1C1C1F` | Карточки, модалки |
| `border` | `#2A2A2E` | Разделители, рамки |
| `text-primary` | `#FFFFFF` | Заголовки, основной текст |
| `text-secondary` | `#9CA3AF` | Подписи, вторичный текст |
| `accent-red` | `#DC2626` | Основной акцент |
| `accent-red-hover` | `#EF4444` | Hover кнопок |
| `accent-glow` | `rgba(220, 38, 38, 0.35)` | Свечение |
| `success` | `#16A34A` | Approve |
| `attack` | `#DC2626` | Attack side |
| `defense` | `#2563EB` | Defense side |

---

## 💡 Советы по использованию

1. **Figma AI (Make Design):** вставьте английский промпт целиком. Если результат слишком "общий", добавьте фразу: *"Rainbow Six Siege tactical UI style, dark dashboard, high contrast"*.
2. **Для ручной работы:** используйте токены из таблицы выше и компоненты, описанные в промпте.
3. **Адаптация под Vue:** текущий проект использует Vue 3 + Vite, поэтому дизайн должен быть компонентно-ориентированным (кнопки, таблицы, карточки — отдельные компоненты).
4. **Backend-контекст:** админ-панель работает с ASP.NET Core API. Основные сущности: Strats, Categories, Operators, Maps, Submissions. Подробнее см. `Back/MapifyBackend/API_DOCUMENTATION.md`.
