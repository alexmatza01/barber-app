# Deployment Guide

This guide covers various deployment options for the Barber Appointment Application.

## Table of Contents
1. [Docker Deployment](#docker-deployment)
2. [Azure Deployment](#azure-deployment)
3. [Traditional Hosting](#traditional-hosting)
4. [Environment Configuration](#environment-configuration)

---

## Docker Deployment

### Prerequisites
- Docker Desktop or Docker Engine
- Docker Compose

### Quick Start with Docker Compose

1. **Clone the repository**:
   ```bash
   git clone https://github.com/alexmatza01/barber-app.git
   cd barber-app
   ```

2. **Update environment variables** in `docker-compose.yml`:
   - Database password
   - API URLs
   - Google Calendar credentials

3. **Build and run**:
   ```bash
   docker-compose up --build
   ```

4. **Access the application**:
   - Frontend: http://localhost:4200
   - Backend API: http://localhost:5000
   - SQL Server: localhost:1433

### Individual Container Deployment

#### Backend
```bash
docker build -f Dockerfile.backend -t barber-api .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="..." barber-api
```

#### Frontend
```bash
docker build -f Dockerfile.frontend -t barber-frontend .
docker run -p 4200:80 barber-frontend
```

---

## Azure Deployment

### Backend (Azure App Service)

1. **Create Azure App Service**:
   ```bash
   az webapp create --resource-group myResourceGroup \
     --plan myAppServicePlan \
     --name my-barber-api \
     --runtime "DOTNET|9.0"
   ```

2. **Create Azure SQL Database**:
   ```bash
   az sql server create --name my-sql-server \
     --resource-group myResourceGroup \
     --location eastus \
     --admin-user sqladmin \
     --admin-password <YourPassword>
   
   az sql db create --resource-group myResourceGroup \
     --server my-sql-server \
     --name BarberAppDb \
     --service-objective S0
   ```

3. **Configure connection string**:
   ```bash
   az webapp config connection-string set \
     --resource-group myResourceGroup \
     --name my-barber-api \
     --settings DefaultConnection="Server=tcp:my-sql-server.database.windows.net,1433;Database=BarberAppDb;User ID=sqladmin;Password=<YourPassword>;Encrypt=True;" \
     --connection-string-type SQLAzure
   ```

4. **Deploy from Visual Studio or CLI**:
   ```bash
   cd backend/BarberApp.API
   dotnet publish -c Release
   az webapp deploy --resource-group myResourceGroup \
     --name my-barber-api \
     --src-path ./bin/Release/net9.0/publish.zip
   ```

### Frontend (Azure Static Web Apps)

1. **Create Static Web App**:
   ```bash
   az staticwebapp create \
     --name my-barber-frontend \
     --resource-group myResourceGroup \
     --source https://github.com/alexmatza01/barber-app \
     --location "East US 2" \
     --branch main \
     --app-location "/frontend" \
     --output-location "dist/frontend"
   ```

2. **Update API URL** in `frontend/src/app/services/api.service.ts`:
   ```typescript
   private apiUrl = 'https://my-barber-api.azurewebsites.net/api';
   ```

3. **GitHub Actions** will automatically deploy on push to main branch

---

## Traditional Hosting

### Backend Deployment

#### On Windows Server (IIS)

1. **Install prerequisites**:
   - .NET 9.0 Runtime
   - IIS with ASP.NET Core Module
   - SQL Server

2. **Publish application**:
   ```bash
   cd backend/BarberApp.API
   dotnet publish -c Release -o ./publish
   ```

3. **Configure IIS**:
   - Create new Application Pool (.NET CLR Version: No Managed Code)
   - Create new website pointing to publish folder
   - Configure application pool identity for database access

4. **Update connection string** in `appsettings.json`

#### On Linux (with Nginx)

1. **Install .NET Runtime**:
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --runtime aspnetcore --version 9.0
   ```

2. **Create systemd service** (`/etc/systemd/system/barber-api.service`):
   ```ini
   [Unit]
   Description=Barber API Service

   [Service]
   WorkingDirectory=/var/www/barber-api
   ExecStart=/usr/bin/dotnet /var/www/barber-api/BarberApp.API.dll
   Restart=always
   RestartSec=10
   KillSignal=SIGINT
   SyslogIdentifier=barber-api
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production

   [Install]
   WantedBy=multi-user.target
   ```

3. **Configure Nginx**:
   ```nginx
   server {
       listen 80;
       server_name api.yourdomain.com;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }
   ```

4. **Enable and start service**:
   ```bash
   sudo systemctl enable barber-api
   sudo systemctl start barber-api
   ```

### Frontend Deployment

#### Static File Hosting

1. **Build production bundle**:
   ```bash
   cd frontend
   npm run build
   ```

2. **Deploy `dist/frontend` folder** to:
   - **Netlify**: Drag and drop or CLI
   - **Vercel**: Connect GitHub repository
   - **AWS S3 + CloudFront**: Upload to S3 bucket
   - **GitHub Pages**: Use gh-pages package

#### Example: Netlify

```bash
npm install -g netlify-cli
netlify deploy --prod --dir=dist/frontend
```

---

## Environment Configuration

### Production Checklist

#### Backend
- [ ] Update connection string for production database
- [ ] Configure Google Calendar API credentials
- [ ] Enable HTTPS
- [ ] Configure CORS for production domain
- [ ] Set up logging (Application Insights, Serilog)
- [ ] Enable authentication/authorization
- [ ] Configure rate limiting
- [ ] Set up health checks
- [ ] Configure caching
- [ ] Enable compression

#### Frontend
- [ ] Update API URL to production endpoint
- [ ] Configure production environment settings
- [ ] Enable service worker for PWA (optional)
- [ ] Set up analytics (Google Analytics, etc.)
- [ ] Configure error tracking (Sentry, etc.)
- [ ] Optimize images and assets
- [ ] Enable CDN
- [ ] Set up monitoring

### Environment Variables

#### Backend (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Production-Connection-String"
  },
  "GoogleCalendar": {
    "ApiKey": "prod-api-key",
    "CalendarId": "prod-calendar-id"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

#### Frontend (environment.prod.ts)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api'
};
```

---

## SSL/TLS Configuration

### Using Let's Encrypt

1. **Install Certbot**:
   ```bash
   sudo apt-get install certbot python3-certbot-nginx
   ```

2. **Obtain certificate**:
   ```bash
   sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
   ```

3. **Auto-renewal**:
   ```bash
   sudo certbot renew --dry-run
   ```

---

## Database Migration

### Production Database Setup

1. **Create database**:
   ```sql
   CREATE DATABASE BarberAppDb;
   ```

2. **Run migrations** (if using EF Core migrations):
   ```bash
   dotnet ef database update --project backend/BarberApp.API
   ```

3. **Backup strategy**:
   - Set up automated backups
   - Test restore procedures
   - Configure point-in-time recovery

---

## Monitoring and Logging

### Application Insights (Azure)

1. **Add package**:
   ```bash
   dotnet add package Microsoft.ApplicationInsights.AspNetCore
   ```

2. **Configure in Program.cs**:
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

### Health Checks

Add to `Program.cs`:
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<BarberAppDbContext>();

app.MapHealthChecks("/health");
```

---

## Scaling Considerations

### Horizontal Scaling
- Use load balancer (Azure Load Balancer, AWS ELB)
- Ensure session state is stateless or use distributed cache
- Configure database connection pooling

### Caching
- Implement response caching
- Use Redis for distributed caching
- Cache static assets on CDN

### Database Optimization
- Add appropriate indexes
- Use connection pooling
- Consider read replicas for heavy read operations

---

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Check connection string
   - Verify firewall rules
   - Ensure SQL Server is accessible

2. **CORS Errors**:
   - Update CORS policy in backend
   - Check frontend API URL

3. **Google Calendar Integration**:
   - Verify API credentials
   - Check quota limits
   - Ensure calendar ID is correct

### Logs Location
- **Backend**: Check application logs in hosting environment
- **Frontend**: Browser console
- **Docker**: `docker logs <container-name>`

---

## Security Best Practices

1. **Use HTTPS everywhere**
2. **Implement authentication** (JWT, OAuth)
3. **Secure admin endpoints**
4. **Validate all inputs**
5. **Use parameterized queries** (already done with EF Core)
6. **Keep dependencies updated**
7. **Implement rate limiting**
8. **Use security headers**
9. **Regular security audits**
10. **Follow OWASP guidelines**

---

## Support

For deployment issues:
1. Check application logs
2. Review this guide
3. Consult technology-specific documentation
4. Open an issue on GitHub

---

**Happy Deploying! 🚀**
