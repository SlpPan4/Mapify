# 📚 Mapify Backend API Documentation

Полная документация всех API endpoints проекта Mapify.

---

## 📑 Содержание

1. [CategoriesController](#categoriescontroller)
2. [OperatorsController](#operatorscontroller)
3. [StratsController](#stratscontroller)
4. [Коды ответов](#коды-ответов)
5. [Структура данных](#структура-данных)

---

## 🎯 CategoriesController

**Base URL:** `/api/categories`

### GET /api/categories
Получить все категории

**Метод:** `GET`  
**Параметры:** Нет  
**Ответ:**
```
200 OK
Content-Type: application/json

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

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID категории

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "id": 1,
  "name": "Anti-Eco",
  "side": "T"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Category by ID 999 was not found"
}
```

---

### GET /api/categories/category_name/{id}
Получить только имя категории

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID категории

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "name": "Anti-Eco"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Category by id 999 was not found"
}
```

---

### POST /api/categories
Создать новую категорию

**Метод:** `POST`  
**Content-Type:** `application/json`

**Request Body:**
```json
{
  "name": "Anti-Eco",
  "side": "T"
}
```

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Category added"
}
```

**Ответ ошибки:**
```
400 Bad Request
Content-Type: application/json

{
  "error": "Name is required"
}
```

или

```
400 Bad Request
Content-Type: application/json

{
  "error": "Error adding category"
}
```

---

### DELETE /api/categories/{id}
Удалить категорию

**Метод:** `DELETE`  
**Параметры:**
- `id` (int, path) - ID категории

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Category deleted"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Category by id 999 was not found"
}
```

---

## 🎮 OperatorsController

**Base URL:** `/api/operators`

### GET /api/operators
Получить всех операторов

**Метод:** `GET`  
**Параметры:** Нет

**Ответ:**
```
200 OK
Content-Type: application/json

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

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID оператора

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "id": 1,
  "name": "IGL"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Operator not found"
}
```

---

### GET /api/operators/by-name/{name}
Получить ID оператора по имени

**Метод:** `GET`  
**Параметры:**
- `name` (string, path) - Имя оператора

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "id": 1
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Operator not found"
}
```

---

### POST /api/operators/{operatorId}/assign/{stratId}
Назначить оператора к стратегии

**Метод:** `POST`  
**Параметры:**
- `operatorId` (int, path) - ID оператора
- `stratId` (int, path) - ID стратегии

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Operator assigned successfully"
}
```

**Ответ ошибки:**
```
400 Bad Request
Content-Type: application/json

{
  "message": "Could not assign operator to strategy"
}
```

---

### DELETE /api/operators/{operatorId}/remove/{stratId}
Удалить оператора из стратегии

**Метод:** `DELETE`  
**Параметры:**
- `operatorId` (int, path) - ID оператора
- `stratId` (int, path) - ID стратегии

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Operator removed from strategy"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Relation not found"
}
```

---

## 📺 StratsController

**Base URL:** `/api/strats`

### GET /api/strats
Получить все стратегии

**Метод:** `GET`  
**Параметры:** Нет

**Ответ:**
```
200 OK
Content-Type: application/json

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

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID стратегии

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "id": 1,
  "name": "B-Site Execute",
  "videoUrl": "https://youtube.com/watch?v=...",
  "mapName": "Mirage"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Strat by ID 999 was not found"
}
```

---

### GET /api/strats/maps/{id}
Получить карту по ID

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID карты

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "id": 1,
  "name": "Mirage"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Map by id 999 not found"
}
```

или

```
400 Bad Request
Content-Type: application/json

{
  "error": "Exception message"
}
```

---

### GET /api/strats/category/{id}
Получить в��е стратегии категории

**Метод:** `GET`  
**Параметры:**
- `id` (int, path) - ID категории

**Ответ успеха:**
```
200 OK
Content-Type: application/json

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
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "No strats found in category 999"
}
```

---

### POST /api/strats
Создать новую стратегию

**Метод:** `POST`  
**Content-Type:** `application/json`

**Request Body:**
```json
{
  "name": "B-Site Execute",
  "videoUrl": "https://youtube.com/watch?v=...",
  "mapName": "Mirage"
}
```

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Strategy added!"
}
```

**Ответ ошибки:**
```
400 Bad Request
Content-Type: application/json

{
  "error": "Name is required"
}
```

или

```
400 Bad Request
Content-Type: application/json

{
  "error": "Error creating strategy"
}
```

---

### POST /api/strats/{stratId}/{categoryId}
Назначить стратегию категории

**Метод:** `POST`  
**Параметры:**
- `stratId` (int, path) - ID стратегии
- `categoryId` (int, path) - ID категории

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Strat assigned"
}
```

**Ответ ошибки:**
```
400 Bad Request
Content-Type: application/json

{
  "error": "Exception message"
}
```

---

### DELETE /api/strats/{id}
Удалить стратегию

**Метод:** `DELETE`  
**Параметры:**
- `id` (int, path) - ID стратегии

**Ответ успеха:**
```
200 OK
Content-Type: application/json

{
  "message": "Strategy deleted!"
}
```

**Ответ ошибки:**
```
404 Not Found
Content-Type: application/json

{
  "message": "Strat by ID 999 was not found"
}
```

---

## 📊 Коды ответов

| Код | Описание |
|-----|---------|
| `200 OK` | Успешный запрос |
| `400 Bad Request` | Ошибка валидации или неверные данные |
| `404 Not Found` | Ресурс не найден |

---

## 🔧 Структура данных

### Category
```json
{
  "id": 1,
  "name": "string",
  "side": "string"
}
```

### Operator
```json
{
  "id": 1,
  "name": "string"
}
```

### Strat
```json
{
  "id": 1,
  "name": "string",
  "videoUrl": "string",
  "mapName": "string"
}
```

### Map
```json
{
  "id": 1,
  "name": "string"
}
```

---

## 🔗 Быстрые ссылки

### Все GET запросы
- `GET /api/categories` - Все категории
- `GET /api/categories/{id}` - Категория по ID
- `GET /api/categories/category_name/{id}` - Имя категории
- `GET /api/operators` - Все операторы
- `GET /api/operators/{id}` - Оператор по ID
- `GET /api/operators/by-name/{name}` - ID по имени оператора
- `GET /api/strats` - Все стратегии
- `GET /api/strats/{id}` - Стратегия по ID
- `GET /api/strats/maps/{id}` - Карта по ID
- `GET /api/strats/category/{id}` - Стратегии категории

### Все POST запросы
- `POST /api/categories` - Создать категорию
- `POST /api/operators/{operatorId}/assign/{stratId}` - Добавить оператора к стратегии
- `POST /api/strats` - Создать стратегию
- `POST /api/strats/{stratId}/{categoryId}` - Назначить стратегию категории

### Все DELETE запросы
- `DELETE /api/categories/{id}` - Удалить категорию
- `DELETE /api/operators/{operatorId}/remove/{stratId}` - Удалить оператора из стратегии
- `DELETE /api/strats/{id}` - Удалить стратегию

---

**Последнее обновление:** 2026-06-02
