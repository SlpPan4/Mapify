# Mapify Backend API Documentation

Updated: 2026-06-30

Base URL during local development depends on the ASP.NET launch profile, usually `http://localhost:{port}`. All endpoints below are relative to that base URL and return JSON.

## Conventions

- `side` must be `"Attack"` or `"Defense"`.
- Current create/update-style endpoints generally return `200 OK` instead of `201 Created`.
- Admin submission endpoints are route-separated but not protected by backend authentication yet. They must not be exposed publicly without hosting/auth restrictions.
- Public frontend submission endpoints should use `/api/submissions/...`, not the direct `/api/strats` or `/api/categories` create endpoints.

## Data Shapes

### Strat

```json
{
  "id": 1,
  "name": "Cool Ash Rush",
  "videoUrl": "youtube.com",
  "mapId": 7,
  "description": ""
}
```

### Map

```json
{
  "id": 1,
  "name": "Oregon"
}
```

### Category

```json
{
  "id": 1,
  "name": "Rush",
  "side": "Attack"
}
```

### Operator

```json
{
  "id": 1,
  "name": "Ash",
  "side": "Attack"
}
```

### Strat Submission

```json
{
  "id": 1,
  "name": "Oregon attic execute",
  "videoUrl": "https://youtube.com/watch?v=example",
  "mapId": 1,
  "description": "Open attic wall and plant behind half wall.",
  "submittedAt": "2026-06-30T12:00:00",
  "categoryIds": [1, 2],
  "operatorIds": [1, 4]
}
```

### Category Submission

```json
{
  "id": 1,
  "name": "Shield clear",
  "side": "Attack",
  "submittedAt": "2026-06-30T12:00:00"
}
```

## Strats

Base route: `/api/strats`

### Get All Strats

`GET /api/strats`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Cool Ash Rush",
    "videoUrl": "youtube.com",
    "mapId": 7,
    "description": ""
  }
]
```

### Get Strat By ID

`GET /api/strats/{id}`

Response `200 OK`:

```json
{
  "id": 1,
  "name": "Cool Ash Rush",
  "videoUrl": "youtube.com",
  "mapId": 7,
  "description": ""
}
```

Response `404 Not Found`:

```json
{
  "message": "Strat by ID 999 was not found"
}
```

### Create Strat Directly

`POST /api/strats`

Use this only for trusted/internal tools. Public user submissions should go through `/api/submissions/strats`.

Request:

```json
{
  "name": "Oregon rush",
  "videoUrl": "https://youtube.com/watch?v=example",
  "mapName": "Oregon"
}
```

Response `200 OK`:

```json
{
  "message": "Strategy added!"
}
```

Response `400 Bad Request`:

```json
{
  "error": "Name is required"
}
```

Response `404 Not Found`:

```json
{
  "error": "No map by the name Oregon"
}
```

### Delete Strat

`DELETE /api/strats/{id}`

Response `200 OK`:

```json
{
  "message": "Strategy deleted!"
}
```

Response `404 Not Found`:

```json
{
  "message": "Strat by ID 999 was not found"
}
```

### Assign Strat To Category

`POST /api/strats/assign/strat/{stratId}/category/{categoryId}`

Response `200 OK`:

```json
{
  "message": "Strat assigned"
}
```

Response `400 Bad Request`:

```json
{
  "error": "No category by id 999"
}
```

### Get Strats By Category

`GET /api/strats/category/{id}`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Cool Ash Rush",
    "videoUrl": "youtube.com",
    "mapId": 7,
    "description": ""
  }
]
```

Response `404 Not Found`:

```json
{
  "message": "No strats found in category 999"
}
```

### Get Map By ID

`GET /api/strats/maps/{id}`

Response `200 OK`:

```json
{
  "id": 1,
  "name": "Oregon"
}
```

Response `404 Not Found`:

```json
{
  "message": "Map by id 999 not found"
}
```

### Get Map ID By Name

`GET /api/strats/maps/byname/{mapName}`

Response `200 OK`:

```json
1
```

Current behavior may return `200 OK` with `null` for an unknown map.

## Categories

Base route: `/api/categories`

### Get All Categories

`GET /api/categories`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Rush",
    "side": "Attack"
  }
]
```

### Get Category By ID

`GET /api/categories/{id}`

Response `200 OK`:

```json
{
  "id": 1,
  "name": "Rush",
  "side": "Attack"
}
```

Response `404 Not Found`:

```json
{
  "message": "Category by ID 999 was not found"
}
```

### Create Category Directly

`POST /api/categories`

Use this only for trusted/internal tools. Public user submissions should go through `/api/submissions/categories`.

Request:

```json
{
  "name": "Shield clear",
  "side": "Attack"
}
```

Response `200 OK`:

```json
{
  "message": "Category added"
}
```

Response `400 Bad Request`:

```json
{
  "error": "Side must be only 'Attack' or 'Defense'"
}
```

### Delete Category

`DELETE /api/categories/{id}`

Response `200 OK`:

```json
{
  "message": "Category deleted"
}
```

Response `404 Not Found`:

```json
{
  "message": "Category by id 999 was not found"
}
```

### Get Category Name

`GET /api/categories/category_name/{id}`

Response `200 OK`:

```json
{
  "name": "Rush"
}
```

Response `404 Not Found`:

```json
{
  "message": "Category by id 999 was not found"
}
```

## Operators

Base route: `/api/operators`

### Get All Operators

`GET /api/operators`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Ash",
    "side": "Attack"
  }
]
```

### Get Operator By ID

`GET /api/operators/{id}`

Response `200 OK`:

```json
{
  "id": 1,
  "name": "Ash",
  "side": "Attack"
}
```

Response `404 Not Found`:

```json
{
  "message": "Operator not found"
}
```

### Get Operator ID By Name

`GET /api/operators/by-name/{name}`

Response `200 OK`:

```json
{
  "id": 1
}
```

Response `404 Not Found`:

```json
{
  "message": "Operator not found"
}
```

### Assign Operator To Strat

`POST /api/operators/{operatorId}/assign/{stratId}`

Response `200 OK`:

```json
{
  "message": "Operator assigned successfully"
}
```

Response `400 Bad Request`:

```json
{
  "message": "Could not assign operator to strategy"
}
```

### Remove Operator From Strat

`DELETE /api/operators/{operatorId}/remove/{stratId}`

Response `200 OK`:

```json
{
  "message": "Operator removed from strategy"
}
```

Response `404 Not Found`:

```json
{
  "message": "Relation not found"
}
```

## Submissions

Base route: `/api/submissions`

These endpoints store user-provided content in pending tables. Approved content is moved into the normal `strats`, `categories`, `strat_categories`, and `strat_operators` tables.

### Submit Strat For Approval

`POST /api/submissions/strats`

Request:

```json
{
  "name": "Oregon attic execute",
  "videoUrl": "https://youtube.com/watch?v=example",
  "mapName": "Oregon",
  "description": "Open attic wall and plant behind half wall.",
  "categoryIds": [1, 2],
  "operatorIds": [1, 4]
}
```

Response `200 OK`:

```json
{
  "message": "Strategy submitted for approval",
  "submissionId": 1
}
```

Response `400 Bad Request`:

```json
{
  "error": "VideoUrl is required"
}
```

Response `404 Not Found`:

```json
{
  "error": "Map 'Unknown' does not exist"
}
```

### Submit Category For Approval

`POST /api/submissions/categories`

Request:

```json
{
  "name": "Shield clear",
  "side": "Attack"
}
```

Response `200 OK`:

```json
{
  "message": "Category submitted for approval",
  "submissionId": 1
}
```

Response `400 Bad Request`:

```json
{
  "error": "Side must be only 'Attack' or 'Defense'"
}
```

## Admin Submission Review

Base route: `/api/submissions/admin`

These endpoints are intended for a future admin panel. They are not protected by backend auth yet.

### Get Pending Strat Submissions

`GET /api/submissions/admin/strats`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Oregon attic execute",
    "videoUrl": "https://youtube.com/watch?v=example",
    "mapId": 1,
    "description": "Open attic wall and plant behind half wall.",
    "submittedAt": "2026-06-30T12:00:00",
    "categoryIds": [1, 2],
    "operatorIds": [1, 4]
  }
]
```

### Get Pending Strat Submission

`GET /api/submissions/admin/strats/{id}`

Response `200 OK`: a single strat submission.

Response `404 Not Found`:

```json
{
  "message": "Strategy submission by ID 999 was not found"
}
```

### Approve Pending Strat Submission

`POST /api/submissions/admin/strats/{id}/approve`

Response `200 OK`:

```json
{
  "message": "Strategy submission approved",
  "stratId": 4
}
```

Response `404 Not Found`:

```json
{
  "message": "Strategy submission by ID 999 was not found"
}
```

### Reject Pending Strat Submission

`DELETE /api/submissions/admin/strats/{id}`

Response `200 OK`:

```json
{
  "message": "Strategy submission rejected"
}
```

### Get Pending Category Submissions

`GET /api/submissions/admin/categories`

Response `200 OK`:

```json
[
  {
    "id": 1,
    "name": "Shield clear",
    "side": "Attack",
    "submittedAt": "2026-06-30T12:00:00"
  }
]
```

### Get Pending Category Submission

`GET /api/submissions/admin/categories/{id}`

Response `200 OK`: a single category submission.

Response `404 Not Found`:

```json
{
  "message": "Category submission by ID 999 was not found"
}
```

### Approve Pending Category Submission

`POST /api/submissions/admin/categories/{id}/approve`

Response `200 OK`:

```json
{
  "message": "Category submission approved",
  "categoryId": 9
}
```

### Reject Pending Category Submission

`DELETE /api/submissions/admin/categories/{id}`

Response `200 OK`:

```json
{
  "message": "Category submission rejected"
}
```

## Frontend Quick Reference

Read endpoints:

- `GET /api/strats`
- `GET /api/strats/{id}`
- `GET /api/strats/category/{id}`
- `GET /api/strats/maps/{id}`
- `GET /api/strats/maps/byname/{mapName}`
- `GET /api/categories`
- `GET /api/categories/{id}`
- `GET /api/categories/category_name/{id}`
- `GET /api/operators`
- `GET /api/operators/{id}`
- `GET /api/operators/by-name/{name}`

Public submission endpoints:

- `POST /api/submissions/strats`
- `POST /api/submissions/categories`

Trusted/admin mutation endpoints:

- `POST /api/strats`
- `DELETE /api/strats/{id}`
- `POST /api/strats/assign/strat/{stratId}/category/{categoryId}`
- `POST /api/categories`
- `DELETE /api/categories/{id}`
- `POST /api/operators/{operatorId}/assign/{stratId}`
- `DELETE /api/operators/{operatorId}/remove/{stratId}`
- `GET /api/submissions/admin/strats`
- `GET /api/submissions/admin/strats/{id}`
- `POST /api/submissions/admin/strats/{id}/approve`
- `DELETE /api/submissions/admin/strats/{id}`
- `GET /api/submissions/admin/categories`
- `GET /api/submissions/admin/categories/{id}`
- `POST /api/submissions/admin/categories/{id}/approve`
- `DELETE /api/submissions/admin/categories/{id}`
