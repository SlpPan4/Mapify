# Mapify Backend API Documentation

Updated: 2026-07-20

Base URL during local development depends on the ASP.NET launch profile, usually `http://localhost:{port}`. All endpoints below are relative to that base URL and return JSON.

## Conventions

- All responses share a single envelope format:

  ```json
  {
    "status": 200,
    "data": { ... },
    "message": "...",
    "error": null
  }
  ```

  - `status` — HTTP status code of the response.
  - `data` — response payload on success; `null` on error.
  - `message` — human-readable summary; present on many success responses and some errors.
  - `error` — error description; `null` on success.

- `side` must be `"Attack"` or `"Defense"`.
- Create endpoints return `201 Created` with a `Location` header and the created resource ID in `data`.
- Admin submission endpoints are route-separated but not protected by backend authentication yet. They must not be exposed publicly without hosting/auth restrictions.
- Public frontend submission endpoints should use `/api/submissions/...`, not the direct `/api/strats` or `/api/categories` create endpoints.
- When submitting a strategy, all selected `categoryIds` must belong to the same `side`, all selected `operatorIds` must belong to the same `side`, and the two sides must match if both lists are provided.

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

### Strat Detail

Returned by `GET /api/strats/{id}`. Includes the related map, categories, and operators.

```json
{
  "id": 1,
  "name": "Cool Ash Rush",
  "videoUrl": "youtube.com",
  "description": "",
  "map": {
    "id": 7,
    "name": "Coastline"
  },
  "categories": [
    {
      "id": 1,
      "name": "Rush",
      "side": "Attack"
    }
  ],
  "operators": [
    {
      "id": 1,
      "name": "Ash",
      "side": "Attack"
    }
  ]
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

Optional query parameters (can be combined):

- `name` — case-insensitive substring match on strategy name.
- `mapId` — filter by map ID.
- `categoryId` — filter by assigned category ID.
- `operatorId` — filter by assigned operator ID.

Example: `GET /api/strats?name=Ash&mapId=7`

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Cool Ash Rush",
      "videoUrl": "youtube.com",
      "mapId": 7,
      "description": ""
    }
  ],
  "message": null,
  "error": null
}
```

### Get Strat By ID

`GET /api/strats/{id}`

Returns the full strategy details including map, categories, and operators.

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Cool Ash Rush",
    "videoUrl": "youtube.com",
    "description": "",
    "map": {
      "id": 7,
      "name": "Coastline"
    },
    "categories": [
      {
        "id": 1,
        "name": "Rush",
        "side": "Attack"
      }
    ],
    "operators": [
      {
        "id": 1,
        "name": "Ash",
        "side": "Attack"
      }
    ]
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strat by ID 999 was not found"
}
```

### Get Strats Summary

`GET /api/strats/summary`

Returns all strategies enriched with their map, derived side, categories, and operators. Useful for list views in the admin panel.

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Cool Ash Rush",
      "videoUrl": "youtube.com",
      "description": "",
      "map": {
        "id": 7,
        "name": "Coastline"
      },
      "side": "Attack",
      "categories": [
        {
          "id": 1,
          "name": "Rush",
          "side": "Attack"
        }
      ],
      "operators": [
        {
          "id": 1,
          "name": "Ash",
          "side": "Attack"
        }
      ]
    }
  ],
  "message": null,
  "error": null
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
  "mapName": "Oregon",
  "description": "Push through attic and plant behind half wall."
}
```

All fields except `description` are required.

Response `201 Created`:

```json
{
  "status": 201,
  "data": {
    "stratId": 4
  },
  "message": "Strategy added!",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "Name is required"
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "No map by the name Oregon"
}
```

### Update Strat

`PUT /api/strats/{id}`

Fully replaces an existing strategy. All fields are required.

Request:

```json
{
  "name": "Oregon rush",
  "videoUrl": "https://youtube.com/watch?v=example",
  "mapName": "Oregon",
  "description": "Updated description"
}
```

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Strategy updated!",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strat by ID 999 was not found"
}
```

### Patch Strat

`PATCH /api/strats/{id}`

Partially updates an existing strategy. Only provided fields are changed.

Request:

```json
{
  "description": "Updated description only"
}
```

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Strategy updated!",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strat by ID 999 was not found"
}
```

### Delete Strat

`DELETE /api/strats/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Strategy deleted!",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strat by ID 999 was not found"
}
```

### Assign Strat To Category

`POST /api/strats/assign/strat/{stratId}/category/{categoryId}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Strat assigned",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "No category by id 999"
}
```

### Remove Category From Strat

`DELETE /api/strats/{stratId}/categories/{categoryId}`

Removes a category assignment from a strategy.

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Category removed from strat",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category assignment not found"
}
```

### Get Strats By Category

`GET /api/strats/category/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Cool Ash Rush",
      "videoUrl": "youtube.com",
      "mapId": 7,
      "description": ""
    }
  ],
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "No strats found in category 999"
}
```

### Get Map By ID

`GET /api/strats/maps/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Oregon"
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Map by id 999 not found"
}
```

### Get Map ID By Name

`GET /api/strats/maps/byname/{mapName}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": 1,
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "No maps found with such name Unknown"
}
```

## Maps

Base route: `/api/maps`

### Get All Maps

`GET /api/maps`

Returns every map in the system.

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Oregon"
    },
    {
      "id": 2,
      "name": "Bank"
    }
  ],
  "message": null,
  "error": null
}
```

## Categories

Base route: `/api/categories`

### Get All Categories

`GET /api/categories`

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Rush",
      "side": "Attack"
    }
  ],
  "message": null,
  "error": null
}
```

### Get Category By ID

`GET /api/categories/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Rush",
    "side": "Attack"
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category by ID 999 was not found"
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

Response `201 Created`:

```json
{
  "status": 201,
  "data": {
    "categoryId": 9
  },
  "message": "Category added",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "Side must be only 'Attack' or 'Defense'"
}
```

### Delete Category

`DELETE /api/categories/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Category deleted",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category by id 999 was not found"
}
```

### Get Category Name

`GET /api/categories/category_name/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "name": "Rush"
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category by id 999 was not found"
}
```

## Operators

Base route: `/api/operators`

### Get All Operators

`GET /api/operators`

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Ash",
      "side": "Attack"
    }
  ],
  "message": null,
  "error": null
}
```

### Get Operator By ID

`GET /api/operators/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Ash",
    "side": "Attack"
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Operator not found"
}
```

### Get Operator ID By Name

`GET /api/operators/by-name/{name}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Operator not found"
}
```

### Assign Operator To Strat

`POST /api/operators/{operatorId}/assign/{stratId}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Operator assigned successfully",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "Could not assign operator to strategy"
}
```

### Remove Operator From Strat

`DELETE /api/operators/{operatorId}/remove/{stratId}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Operator removed from strategy",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Relation not found"
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
  "mapId": 1,
  "description": "Open attic wall and plant behind half wall.",
  "categoryIds": [1, 2],
  "operatorIds": [1, 4]
}
```

Response `201 Created`:

```json
{
  "status": 201,
  "data": {
    "submissionId": 1
  },
  "message": "Strategy submitted for approval",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "VideoUrl is required"
}
```

Returned when selected categories or operators have mismatched sides:

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "All selected categories must belong to the same side"
}
```

```json
{
  "status": 400,
  "data": null,
  "message": null,
  "error": "Selected operators belong to Attack, but selected categories belong to Defense"
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Map by id 999 does not exist"
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

Response `201 Created`:

```json
{
  "status": 201,
  "data": {
    "submissionId": 1
  },
  "message": "Category submitted for approval",
  "error": null
}
```

Response `400 Bad Request`:

```json
{
  "status": 400,
  "data": null,
  "message": null,
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
{
  "status": 200,
  "data": [
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
  ],
  "message": null,
  "error": null
}
```

### Get Pending Strat Submission

`GET /api/submissions/admin/strats/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Oregon attic execute",
    "videoUrl": "https://youtube.com/watch?v=example",
    "mapId": 1,
    "description": "Open attic wall and plant behind half wall.",
    "submittedAt": "2026-06-30T12:00:00",
    "categoryIds": [1, 2],
    "operatorIds": [1, 4]
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strategy submission by ID 999 was not found"
}
```

### Approve Pending Strat Submission

`POST /api/submissions/admin/strats/{id}/approve`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "stratId": 4
  },
  "message": "Strategy submission approved",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strategy submission by ID 999 was not found"
}
```

### Reject Pending Strat Submission

`DELETE /api/submissions/admin/strats/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Strategy submission rejected",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Strategy submission by ID 999 was not found"
}
```

### Get Pending Category Submissions

`GET /api/submissions/admin/categories`

Response `200 OK`:

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Shield clear",
      "side": "Attack",
      "submittedAt": "2026-06-30T12:00:00"
    }
  ],
  "message": null,
  "error": null
}
```

### Get Pending Category Submission

`GET /api/submissions/admin/categories/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Shield clear",
    "side": "Attack",
    "submittedAt": "2026-06-30T12:00:00"
  },
  "message": null,
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category submission by ID 999 was not found"
}
```

### Approve Pending Category Submission

`POST /api/submissions/admin/categories/{id}/approve`

Response `200 OK`:

```json
{
  "status": 200,
  "data": {
    "categoryId": 9
  },
  "message": "Category submission approved",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category submission by ID 999 was not found"
}
```

### Reject Pending Category Submission

`DELETE /api/submissions/admin/categories/{id}`

Response `200 OK`:

```json
{
  "status": 200,
  "data": null,
  "message": "Category submission rejected",
  "error": null
}
```

Response `404 Not Found`:

```json
{
  "status": 404,
  "data": null,
  "message": null,
  "error": "Category submission by ID 999 was not found"
}
```

## Frontend Quick Reference

Read endpoints:

- `GET /api/strats`
- `GET /api/strats/{id}`
- `GET /api/strats/summary`
- `GET /api/strats/category/{id}`
- `GET /api/strats/maps/{id}`
- `GET /api/strats/maps/byname/{mapName}`
- `GET /api/maps`
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
- `PUT /api/strats/{id}`
- `PATCH /api/strats/{id}`
- `DELETE /api/strats/{id}`
- `POST /api/strats/assign/strat/{stratId}/category/{categoryId}`
- `DELETE /api/strats/{stratId}/categories/{categoryId}`
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
