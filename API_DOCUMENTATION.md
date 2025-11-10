# API Documentation

## Base URL
```
http://localhost:5000/api
```

## Authentication
Currently, the API does not require authentication for most endpoints. Admin endpoints should be protected in production.

---

## Barber Info Endpoints

### Get Barber Information
Returns the barber shop's basic information.

**Endpoint:** `GET /barberinfo`

**Response:**
```json
{
  "id": 1,
  "name": "John's Barber Shop",
  "bio": "Professional barbering services...",
  "profileImage": "https://example.com/image.jpg",
  "phone": "(555) 123-4567",
  "email": "john@barbershop.com",
  "address": "123 Main Street, City, State 12345",
  "googleCalendarId": "calendar-id@group.calendar.google.com",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Update Barber Information (Admin)
Updates barber shop information.

**Endpoint:** `PUT /admin/barber-info`

**Request Body:**
```json
{
  "name": "Updated Shop Name",
  "bio": "Updated bio",
  "profileImage": "https://example.com/new-image.jpg",
  "phone": "(555) 987-6543",
  "email": "newemail@barbershop.com",
  "address": "456 New Street",
  "googleCalendarId": "new-calendar-id"
}
```

---

## Portfolio Endpoints

### Get All Portfolio Items
Returns all portfolio items ordered by display order.

**Endpoint:** `GET /portfolio`

**Response:**
```json
[
  {
    "id": 1,
    "title": "Classic Fade",
    "description": "A perfect fade haircut",
    "imageUrl": "https://example.com/fade.jpg",
    "order": 1,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

### Get Portfolio Item
Returns a specific portfolio item.

**Endpoint:** `GET /portfolio/{id}`

### Create Portfolio Item (Admin)
Creates a new portfolio item.

**Endpoint:** `POST /admin/portfolio`

**Request Body:**
```json
{
  "title": "New Style",
  "description": "Description of the style",
  "imageUrl": "https://example.com/style.jpg",
  "order": 10
}
```

### Update Portfolio Item (Admin)
Updates an existing portfolio item.

**Endpoint:** `PUT /admin/portfolio/{id}`

**Request Body:**
```json
{
  "title": "Updated Title",
  "description": "Updated description",
  "imageUrl": "https://example.com/updated.jpg",
  "order": 5
}
```

### Delete Portfolio Item (Admin)
Deletes a portfolio item.

**Endpoint:** `DELETE /admin/portfolio/{id}`

---

## Service Types Endpoints

### Get All Service Types
Returns all active service types.

**Endpoint:** `GET /servicetypes`

**Response:**
```json
[
  {
    "id": 1,
    "name": "Haircut",
    "description": "Classic haircut",
    "price": 25.00,
    "durationMinutes": 30,
    "isActive": true
  }
]
```

### Get Service Type
Returns a specific service type.

**Endpoint:** `GET /servicetypes/{id}`

### Create Service Type (Admin)
Creates a new service type.

**Endpoint:** `POST /admin/service-types`

**Request Body:**
```json
{
  "name": "Deluxe Cut",
  "description": "Premium haircut with styling",
  "price": 45.00,
  "durationMinutes": 60
}
```

### Update Service Type (Admin)
Updates an existing service type.

**Endpoint:** `PUT /admin/service-types/{id}`

**Request Body:**
```json
{
  "name": "Updated Service",
  "description": "Updated description",
  "price": 30.00,
  "durationMinutes": 45,
  "isActive": true
}
```

### Delete Service Type (Admin)
Deactivates a service type (soft delete).

**Endpoint:** `DELETE /admin/service-types/{id}`

---

## Appointments Endpoints

### Get Appointments
Returns appointments with optional date filtering.

**Endpoint:** `GET /appointments`

**Query Parameters:**
- `startDate` (optional): ISO 8601 date string
- `endDate` (optional): ISO 8601 date string

**Response:**
```json
[
  {
    "id": 1,
    "customerName": "Jane Doe",
    "customerEmail": "jane@example.com",
    "customerPhone": "(555) 123-4567",
    "startTime": "2024-01-15T10:00:00Z",
    "endTime": "2024-01-15T10:30:00Z",
    "serviceType": "Haircut",
    "notes": "Prefer shorter sides",
    "status": "Confirmed",
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

### Get Appointment
Returns a specific appointment.

**Endpoint:** `GET /appointments/{id}`

### Create Appointment
Creates a new appointment.

**Endpoint:** `POST /appointments`

**Request Body:**
```json
{
  "customerName": "Jane Doe",
  "customerEmail": "jane@example.com",
  "customerPhone": "(555) 123-4567",
  "startTime": "2024-01-15T10:00:00Z",
  "serviceTypeId": 1,
  "notes": "Special instructions"
}
```

**Response:** Returns the created appointment with calculated end time.

### Update Appointment
Updates an existing appointment.

**Endpoint:** `PUT /appointments/{id}`

**Request Body:**
```json
{
  "startTime": "2024-01-15T11:00:00Z",
  "notes": "Updated notes",
  "status": "Confirmed"
}
```

**Note:** Status values: `Pending`, `Confirmed`, `Cancelled`, `Completed`

### Delete Appointment
Deletes an appointment and removes it from Google Calendar.

**Endpoint:** `DELETE /appointments/{id}`

### Get Available Slots
Returns available time slots for a specific date and service.

**Endpoint:** `GET /appointments/available-slots`

**Query Parameters:**
- `date` (required): ISO 8601 date string
- `serviceTypeId` (required): Service type ID

**Response:**
```json
[
  {
    "startTime": "2024-01-15T09:00:00Z",
    "endTime": "2024-01-15T09:30:00Z"
  },
  {
    "startTime": "2024-01-15T09:30:00Z",
    "endTime": "2024-01-15T10:00:00Z"
  }
]
```

---

## Error Responses

All endpoints return appropriate HTTP status codes:

- `200 OK` - Successful request
- `201 Created` - Resource created successfully
- `204 No Content` - Successful request with no content to return
- `400 Bad Request` - Invalid request data
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

**Error Response Format:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "field": ["Error message"]
  }
}
```

---

## Google Calendar Integration

The API integrates with Google Calendar for appointment management:

1. When an appointment is created, it's automatically added to the configured Google Calendar
2. The event includes customer details and appointment information
3. When an appointment is deleted, it's removed from Google Calendar
4. Available slots are calculated based on existing calendar events

### Configuration

Update `appsettings.json` with your Google Calendar credentials:

```json
{
  "GoogleCalendar": {
    "ApiKey": "your-google-api-key",
    "CredentialPath": "path/to/credentials.json",
    "CalendarId": "your-calendar-id@group.calendar.google.com"
  }
}
```

### Business Hours

Default business hours: 9:00 AM - 6:00 PM
Time slots are generated based on service duration.

---

## Rate Limiting

Currently, there is no rate limiting implemented. For production:
- Implement rate limiting middleware
- Consider using API keys for client identification
- Set appropriate limits per endpoint

---

## CORS Configuration

The API is configured to accept requests from:
- `http://localhost:4200` (Angular development server)

Update `Program.cs` to add additional origins for production.

---

## Best Practices

1. **Date Handling**: Always send dates in ISO 8601 format
2. **Timezones**: All times are in UTC; convert to local time in the frontend
3. **Validation**: The API validates all input; check error messages for details
4. **Idempotency**: Use appropriate HTTP methods (POST for create, PUT for update)
5. **Pagination**: Currently not implemented; may be added for large datasets

---

## Testing

Use tools like:
- **Postman** or **Insomnia** for API testing
- **Swagger UI** - Available at `/swagger` in development mode
- **curl** for command-line testing

Example curl request:
```bash
curl -X GET "http://localhost:5000/api/servicetypes" \
  -H "Accept: application/json"
```
