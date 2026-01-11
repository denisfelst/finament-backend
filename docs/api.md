# API Reference

Base URL: `/api`

All endpoints except auth require a valid JWT token in the `Authorization` header:
```
Authorization: Bearer <token>
```

## Authentication

### POST /auth/login

Authenticate and receive a JWT token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "user@example.com"
  }
}
```

### GET /auth/health

Health check endpoint.

**Response:**
```json
{
  "status": "healthy"
}
```

---

## Categories

### GET /categories

Get all categories for the authenticated user.

**Response:**
```json
[
  {
    "id": 1,
    "userId": 1,
    "name": "Groceries",
    "monthlyLimit": 500,
    "color": "#4CAF50",
    "createdAt": "2024-01-15T10:30:00Z",
    "expenseCount": 12,
    "totalSpent": 423
  }
]
```

### POST /categories

Create a new category.

**Request:**
```json
{
  "name": "Groceries",
  "monthlyLimit": 500,
  "color": "#4CAF50"
}
```

**Validation:**
- `name`: Required, minimum 3 characters, must be unique per user
- `monthlyLimit`: Required, minimum 1 (rounded to integer)
- `color`: Optional, hex format (#RRGGBB), defaults to #FFFFFF

**Response:** Created category object

### PUT /categories/{id}

Update an existing category.

**Request:**
```json
{
  "name": "Food & Groceries",
  "monthlyLimit": 600,
  "color": "#8BC34A"
}
```

**Response:** Updated category object

### DELETE /categories/{id}

Delete a category and all its associated expenses.

**Response:** `204 No Content`

---

## Expenses

### GET /expenses

Get all expenses for the authenticated user.

**Response:**
```json
[
  {
    "id": 1,
    "userId": 1,
    "categoryId": 1,
    "amount": 45,
    "date": "2024-01-20T00:00:00Z",
    "tag": "Weekly shopping",
    "createdAt": "2024-01-20T14:30:00Z"
  }
]
```

### POST /expenses

Create a new expense.

**Request:**
```json
{
  "categoryId": 1,
  "amount": 45,
  "date": "2024-01-20",
  "tag": "Weekly shopping"
}
```

**Validation:**
- `categoryId`: Required, must exist and belong to user
- `amount`: Required, positive integer
- `date`: Required
- `tag`: Optional

**Response:** Created expense object

### PUT /expenses/{id}

Update an existing expense.

**Request:**
```json
{
  "categoryId": 1,
  "amount": 50,
  "date": "2024-01-20",
  "tag": "Updated description"
}
```

**Response:** Updated expense object

### DELETE /expenses/{id}

Delete an expense.

**Response:** `204 No Content`

---

## Settings

### GET /settings

Get settings for the authenticated user.

**Response:**
```json
{
  "id": 1,
  "userId": 1,
  "currency": "USD",
  "cycleStartDay": 1
}
```

### PUT /settings

Create or update user settings.

**Request:**
```json
{
  "currency": "EUR",
  "cycleStartDay": 15
}
```

**Fields:**
- `currency`: Currency code (e.g., USD, EUR)
- `cycleStartDay`: Day of month when billing cycle starts (1-31)

**Response:** Updated settings object

---

## Error Responses

All errors follow this format:

```json
{
  "message": "Error description"
}
```

**Status Codes:**
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - Resource not found
- `409 Conflict` - Business rule violation (e.g., duplicate name)
- `500 Internal Server Error` - Unexpected error