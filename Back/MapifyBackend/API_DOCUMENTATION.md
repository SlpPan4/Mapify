# Mapify Backend API Documentation

Полная документация всех API endpoints проекта Mapify.

---

## Содержание

1. [CategoriesController](#categoriescontroller)
2. [OperatorsController](#operatorscontroller)
3. [StratsController](#stratscontroller)
4. [HTTP Status Codes](#http-status-codes)
5. [Data Structures](#data-structures)
6. [Quick Reference](#quick-reference)

---

## CategoriesController

**Base URL:** `/api/categories`

### GET /api/categories
Получить все категории

```
GET /api/categories

200 OK
[
  {
    "id": 1,
    "name": "Anti-Eco",
    "side": "T"
  },
  {
    "id": 2,
    "name": "Full Buy",
    "side": "CT"
  }
]
```

---

### GET /api/categories/{id}
Получить категорию по ID

```
GET /api/categories/1

200 OK
{
  "id": 1,
  "name": "Anti-Eco",
  "side": "T"
}

404 Not Found
{
  "message": "Category by ID 999 was not found"
}
```

---

### GET /api/categories/category_name/{id}
Получить только имя категории

```
GET /api/categories/category_name/1

200 OK
{
  "name": "Anti-Eco"
}

404 Not Found
{
  "message": "Category by id 999 was not found"
}
```

---

### POST /api/categories
Создать новую категорию

```
POST /api/categories
Content-Type: application/json

{
  "name": "Anti-Eco",
  "side": "T"
}

200 OK
{
  "message": "Category added"
}

400 Bad Request
{
  "error": "Name is required"
}
```

---

### DELETE /api/categories/{id}
Удалить категорию

```
DELETE /api/categories/1

200 OK
{
  "message": "Category deleted"
}

404 Not Found
{
  "message": "Category by id 999 was not found"
}
```

---

## OperatorsController

**Base URL:** `/api/operators`

### GET /api/operators
Получить всех операторов

```
GET /api/operators

200 OK
[
  {
    "id": 1,
    "name": "IGL"
  },
  {
    "id": 2,
    "name": "AWP"
  }
]
```

---

### GET /api/operators/{id}
Получить оператора по ID

```
GET /api/operators/1

200 OK
{
  "id": 1,
  "name": "IGL"
}

404 Not Found
{
  "message": "Operator not found"
}
```

---

### GET /api/operators/by-name/{name}
Получить ID оператора по имени

```
GET /api/operators/by-name/IGL

200 OK
{
  "id": 1
}

404 Not Found
{
  "message": "Operator not found"
}
```

---

### POST /api/operators/{operatorId}/assign/{stratId}
Назначить оператора к стратегии

```
POST /api/operators/1/assign/5

200 OK
{
  "message": "Operator assigned successfully"
}

400 Bad Request
{
  "message": "Could not assign operator to strategy"
}
```

---

### DELETE /api/operators/{operatorId}/remove/{stratId}
Удалить оператора из стратегии

```
DELETE /api/operators/1/remove/5

200 OK
{
  "message": "Operator removed from strategy"
}

404 Not Found
{
  "message": "Relation not found"
}
```

---

## StratsController

**Base URL:** `/api/strats`

### GET /api/strats
Получить все стратегии

```
GET /api/strats

200 OK
[
  {
    "id": 1,
    "name": "B-Site Execute",
    "videoUrl": "https://youtube.com/watch?v=...",
    "mapName": "Mirage"
  },
  {
    "id": 2,
    "name": "A-Site Fast",
    "videoUrl": "https://youtube.com/watch?v=...",
    "mapName": "Inferno"
  }
]
```

---

### GET /api/strats/{id}
Получить стратегию по ID

```
GET /api/strats/1

200 OK
{
  "id": 1,
  "name": "B-Site Execute",
  "videoUrl": "https://youtube.com/watch?v=...",
  "mapName": "Mirage"
}

404 Not Found
{
  "message": "Strat by ID 999 was not found"
}
```

---

### GET /api/strats/maps/{id}
Получить карту по ID

```
GET /api/strats/maps/1

200 OK
{
  "id": 1,
  "name": "Mirage"
}

404 Not Found
{
  "message": "Map by id 999 not found"
}
```

---

### GET /api/strats/map-by-name/{mapName}
Получить ID карты по названию

```
GET /api/strats/map-by-name/Mirage

200 OK
{
  "id": 1
}

404 Not Found
{
  "message": "Map 'Mirage' not found"
}
```

---

### GET /api/strats/category/{id}
Получить все стратегии категории

```
GET /api/strats/category/1

200 OK
[
  {
    "id": 1,
    "name": "B-Site Execute",
    "videoUrl": "https://youtube.com/watch?v=...",
    "mapName": "Mirage"
  },
  {
    "id": 3,
    "name": "B-Site Stack",
    "videoUrl": "https://youtube.com/watch?v=...",
    "mapName": "Mirage"
  }
]

404 Not Found
{
  "message": "No strats found in category 999"
}
```

---

### POST /api/strats
Создать новую стратегию

```
POST /api/strats
Content-Type: application/json

{
  "name": "B-Site Execute",
  "videoUrl": "https://youtube.com/watch?v=...",
  "mapName": "Mirage"
}

200 OK
{
  "message": "Strategy added!"
}

400 Bad Request
{
  "error": "Name is required"
}
```

---

### POST /api/strats/{stratId}/{categoryId}
Назначить стратегию категории

```
POST /api/strats/1/2

200 OK
{
  "message": "Strat assigned"
}

400 Bad Request
{
  "error": "Exception message"
}
```

---

### DELETE /api/strats/{id}
Удалить стратегию

```
DELETE /api/strats/1

200 OK
{
  "message": "Strategy deleted!"
}

404 Not Found
{
  "message": "Strat by ID 999 was not found"
}
```

---

## HTTP Status Codes

| Code | Description |
|------|-------------|
| `200 OK` | Успешный запрос |
| `400 Bad Request` | Ошибка валидации или неверные данные |
| `404 Not Found` | Ресурс не найден |

---

## Data Structures

### Category
```json
{
  "id": 1,
  "name": "Anti-Eco",
  "side": "T"
}
```

### Operator
```json
{
  "id": 1,
  "name": "IGL"
}
```

### Strat
```json
{
  "id": 1,
  "name": "B-Site Execute",
  "videoUrl": "https://youtube.com/watch?v=...",
  "mapName": "Mirage"
}
```

### Map
```json
{
  "id": 1,
  "name": "Mirage"
}
```

---

## Quick Reference

### GET Endpoints
- `/api/categories` — Все категории
- `/api/categories/{id}` — Категория по ID
- `/api/categories/category_name/{id}` — Имя категории
- `/api/operators` — Все операторы
- `/api/operators/{id}` — Оператор по ID
- `/api/operators/by-name/{name}` — ID по имени оператора
- `/api/strats` — Все стратегии
- `/api/strats/{id}` — Стратегия по ID
- `/api/strats/maps/{id}` — Карта по ID
- `/api/strats/map-by-name/{mapName}` — **ID карты по названию** (новое!)
- `/api/strats/category/{id}` — Стратегии категории

### POST Endpoints
- `/api/categories` — Создать категорию
- `/api/operators/{operatorId}/assign/{stratId}` — Добавить оператора к стратегии
- `/api/strats` — Создать стратегию
- `/api/strats/{stratId}/{categoryId}` — Назначить стратегию категории

### DELETE Endpoints
- `/api/categories/{id}` — Удалить категорию
- `/api/operators/{operatorId}/remove/{stratId}` — Удалить оператора из стратегии
- `/api/strats/{id}` — Удалить стратегию

---

**Updated:** 2026-06-07
