# Barber Appointment Application

A complete full-stack web application for managing a barber shop, including portfolio showcase, appointment booking, and admin management features.

## 🚀 Features

### Customer Features
- **Home Page**: Display barber shop information, contact details, and services
- **Portfolio Gallery**: Browse barber's work with images and descriptions
- **Appointment Booking**: 
  - View available time slots
  - Select services with pricing and duration
  - Book appointments with automatic Google Calendar integration
  - Receive email confirmations

### Admin Features
- **Barber Information Management**: Update shop details, contact info, and Google Calendar settings
- **Portfolio Management**: Add, edit, and delete portfolio items
- **Service Management**: Create and manage service types with pricing and duration
- **Appointment Management**: View, update status, and manage customer appointments

## 🛠️ Technology Stack

### Backend
- **.NET 9.0** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Google Calendar API** - Calendar integration
- **JWT Bearer Authentication** - Security (ready for implementation)

### Frontend
- **Angular 20+** - SPA Framework
- **TypeScript** - Type-safe JavaScript
- **CSS3** - Styling
- **RxJS** - Reactive programming

## 📋 Prerequisites

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 20+** and **npm** - [Download](https://nodejs.org/)
- **SQL Server** or **SQL Server LocalDB**
- **Google Calendar API credentials** (optional, for calendar integration)

## 🔧 Setup Instructions

### Backend Setup

1. **Navigate to backend directory**:
   ```bash
   cd backend/BarberApp.API
   ```

2. **Update database connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BarberAppDb;Trusted_Connection=true"
   }
   ```

3. **Configure Google Calendar** (optional):
   - Create a project in [Google Cloud Console](https://console.cloud.google.com/)
   - Enable Google Calendar API
   - Create API credentials
   - Update `appsettings.json`:
     ```json
     "GoogleCalendar": {
       "ApiKey": "your-api-key",
       "CredentialPath": "path-to-credentials.json",
       "CalendarId": "your-calendar-id"
     }
     ```

4. **Install dependencies and run**:
   ```bash
   dotnet restore
   dotnet run
   ```
   
   The API will be available at `http://localhost:5000`

### Frontend Setup

1. **Navigate to frontend directory**:
   ```bash
   cd frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Update API URL** (if needed) in `src/app/services/api.service.ts`:
   ```typescript
   private apiUrl = 'http://localhost:5000/api';
   ```

4. **Run development server**:
   ```bash
   npm start
   ```
   
   The application will be available at `http://localhost:4200`

### Database Initialization

The database is automatically created and seeded with sample data on first run, including:
- Default barber shop information
- Three service types (Haircut, Beard Trim, Hair & Beard)

## 📚 API Endpoints

### Public Endpoints

#### Barber Info
- `GET /api/barberinfo` - Get barber shop information

#### Portfolio
- `GET /api/portfolio` - Get all portfolio items
- `GET /api/portfolio/{id}` - Get specific portfolio item

#### Service Types
- `GET /api/servicetypes` - Get all active service types
- `GET /api/servicetypes/{id}` - Get specific service type

#### Appointments
- `GET /api/appointments` - Get appointments (with optional date filters)
- `GET /api/appointments/{id}` - Get specific appointment
- `POST /api/appointments` - Create new appointment
- `GET /api/appointments/available-slots` - Get available time slots

### Admin Endpoints

#### Barber Info
- `PUT /api/admin/barber-info` - Update barber information

#### Portfolio
- `POST /api/admin/portfolio` - Create portfolio item
- `PUT /api/admin/portfolio/{id}` - Update portfolio item
- `DELETE /api/admin/portfolio/{id}` - Delete portfolio item

#### Service Types
- `POST /api/admin/service-types` - Create service type
- `PUT /api/admin/service-types/{id}` - Update service type
- `DELETE /api/admin/service-types/{id}` - Deactivate service type

#### Appointments
- `PUT /api/appointments/{id}` - Update appointment
- `DELETE /api/appointments/{id}` - Delete appointment

## 🗂️ Project Structure

```
barber-app/
├── backend/
│   └── BarberApp.API/
│       ├── Controllers/        # API endpoints
│       ├── Data/              # Database context
│       ├── Models/            # Domain models
│       ├── DTOs/              # Data transfer objects
│       ├── Services/          # Business logic services
│       └── Program.cs         # Application entry point
│
└── frontend/
    └── src/
        └── app/
            ├── components/    # Angular components
            │   ├── home/
            │   ├── portfolio/
            │   ├── booking/
            │   └── admin/
            ├── services/      # API services
            ├── models/        # TypeScript interfaces
            └── app.routes.ts  # Routing configuration
```

## 🎨 UI Features

- **Responsive Design**: Works on desktop, tablet, and mobile
- **Modern UI**: Clean and professional interface
- **Real-time Updates**: Instant feedback on user actions
- **Error Handling**: User-friendly error messages
- **Loading States**: Visual feedback during data operations

## 🔐 Security Notes

- The current implementation uses CORS configuration for development
- JWT authentication infrastructure is included but not enforced
- For production:
  - Implement proper authentication and authorization
  - Secure admin endpoints
  - Use HTTPS
  - Implement rate limiting
  - Add input validation and sanitization

## 🚀 Deployment

### Backend Deployment
1. Update connection string for production database
2. Configure Google Calendar API credentials
3. Set environment-specific settings
4. Deploy to Azure App Service, AWS, or your preferred hosting

### Frontend Deployment
1. Build for production:
   ```bash
   npm run build
   ```
2. Deploy the `dist/` folder to:
   - Azure Static Web Apps
   - AWS S3 + CloudFront
   - Netlify, Vercel, or similar

## 📝 Future Enhancements

- [ ] Email notifications for appointments
- [ ] SMS reminders
- [ ] Customer accounts and booking history
- [ ] Payment integration
- [ ] Staff management (multiple barbers)
- [ ] Reviews and ratings
- [ ] Push notifications
- [ ] Mobile app (PWA or native)

## 🤝 Contributing

This is a demonstration project. Feel free to fork and customize for your needs.

## 📄 License

This project is provided as-is for educational and commercial use.

## 📞 Support

For issues or questions, please open an issue in the repository.

---

**Made with ❤️ for barbers and their customers**
