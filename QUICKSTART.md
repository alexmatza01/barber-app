# Quick Start Guide - Running Locally

This guide will help you run the Barber Appointment Application locally on your machine using Visual Studio and Visual Studio Code.

## Prerequisites

Before starting, make sure you have installed:

1. **Visual Studio 2022** (for backend)
   - Download from: https://visualstudio.microsoft.com/downloads/
   - Install the "ASP.NET and web development" workload

2. **Visual Studio Code** (for frontend)
   - Download from: https://code.visualstudio.com/

3. **Node.js 20+ and npm**
   - Download from: https://nodejs.org/
   - Verify installation: Open terminal and run `node --version` and `npm --version`

4. **SQL Server LocalDB** (comes with Visual Studio)
   - Or install SQL Server Express from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

---

## Step-by-Step Instructions

### STEP 1: Start the Backend (.NET API)

#### Option A: Using Visual Studio (Recommended)

1. **Open the Backend Project**
   - Launch Visual Studio 2022
   - Click "Open a project or solution"
   - Navigate to: `barber-app/backend/BarberApp.API/BarberApp.API.csproj`
   - Click "Open"

2. **Configure the Launch Settings**
   - The project will open and restore NuGet packages automatically (wait for this to complete)
   - Look at the top toolbar where it shows the run button
   - You should see "http" or "https" profile selected
   - The backend will run on **http://localhost:5241** (or https://localhost:7158)

3. **Run the Backend**
   - Click the green "Start" button (▶) or press `F5`
   - A console window will open showing the backend is running
   - You should see output like:
     ```
     info: Microsoft.Hosting.Lifetime[14]
           Now listening on: http://localhost:5241
     info: Microsoft.Hosting.Lifetime[0]
           Application started. Press Ctrl+C to shut down.
     ```
   - **Keep this window open** - the backend must stay running

4. **Verify Backend is Running**
   - Open your browser and go to: http://localhost:5241/api/servicetypes
   - You should see JSON data with three service types (Haircut, Beard Trim, Hair & Beard)

#### Option B: Using Command Line

1. Open a terminal/command prompt
2. Navigate to the backend directory:
   ```bash
   cd barber-app/backend/BarberApp.API
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the backend:
   ```bash
   dotnet run
   ```
5. Keep this terminal window open

---

### STEP 2: Update Frontend API URL

**IMPORTANT:** Before running the frontend, we need to update the API URL to match the backend port.

1. **Open the Frontend in VS Code**
   - Launch Visual Studio Code
   - Click "File" → "Open Folder"
   - Navigate to: `barber-app/frontend`
   - Click "Select Folder"

2. **Update the API Service**
   - In VS Code's Explorer panel (left side), navigate to:
     `src/app/services/api.service.ts`
   - Find line 17 which says:
     ```typescript
     private apiUrl = 'http://localhost:5000/api';
     ```
   - Change it to:
     ```typescript
     private apiUrl = 'http://localhost:5241/api';
     ```
   - Save the file (Ctrl+S or Cmd+S)

---

### STEP 3: Start the Frontend (Angular)

1. **Open Terminal in VS Code**
   - In VS Code, click "Terminal" → "New Terminal" (or press Ctrl+` )
   - Make sure you're in the `frontend` directory

2. **Install Dependencies** (First time only)
   ```bash
   npm install
   ```
   - This will take a few minutes to download all packages
   - Wait until you see "added XXX packages" message

3. **Start the Development Server**
   ```bash
   npm start
   ```
   - This will compile the Angular application
   - Wait for the message:
     ```
     ✔ Browser application bundle generation complete.
     Initial chunk files | Names | Raw size
     ...
     Application bundle generation complete.
     ```
   - You should see: `Angular Live Development Server is listening on localhost:4200`

4. **Access the Application**
   - Open your browser
   - Go to: **http://localhost:4200**
   - You should see the Barber Shop homepage!

---

## What You Should See

### Homepage (http://localhost:4200)
- Navigation bar with: Home, Portfolio, Book Appointment, Admin
- Hero section with shop name and tagline
- Shop information cards (phone, email, address)
- Feature cards

### Portfolio Page (http://localhost:4200/portfolio)
- Gallery message: "No portfolio items available yet. Check back soon!"
- (Use Admin panel to add portfolio items)

### Booking Page (http://localhost:4200/booking)
- Step 1: Select a service (Haircut, Beard Trim, or Hair & Beard)
- Step 2: Choose a date
- Step 3: Pick a time slot
- Step 4: Enter your information and book

### Admin Page (http://localhost:4200/admin)
- Tabs for: Barber Info, Portfolio, Services, Appointments
- Manage all aspects of the shop

---

## Common Issues and Solutions

### Issue 1: Backend won't start - "Unable to connect to database"
**Solution:**
- Make sure SQL Server LocalDB is installed
- The database will be created automatically on first run
- If using Visual Studio, LocalDB should already be installed

### Issue 2: Frontend shows errors - "Failed to load barber information"
**Solution:**
- Make sure the backend is running (check step 1)
- Verify the API URL in `api.service.ts` matches the backend port (5241)
- Check browser console (F12) for specific error messages

### Issue 3: Port already in use
**Backend (5241):**
```bash
# Find and kill the process
netstat -ano | findstr :5241
taskkill /PID <process_id> /F
```

**Frontend (4200):**
```bash
# Find and kill the process
netstat -ano | findstr :4200
taskkill /PID <process_id> /F
```

### Issue 4: npm install fails
**Solution:**
- Delete `node_modules` folder and `package-lock.json`
- Run `npm install` again
- Make sure you have Node.js 20 or higher

### Issue 5: CORS errors in browser console
**Solution:**
- The backend is already configured for CORS with localhost:4200
- Make sure backend is running first before frontend
- Check that you're accessing frontend via `localhost` not `127.0.0.1`

---

## Stopping the Application

### Stop Backend:
- **Visual Studio**: Click the red "Stop" button (■) or close the console window
- **Command Line**: Press `Ctrl+C` in the terminal

### Stop Frontend:
- In the VS Code terminal, press `Ctrl+C`
- Type `Y` if asked to confirm

---

## Testing the Application

### 1. Test Appointment Booking:
   - Go to "Book Appointment"
   - Select "Haircut" service
   - Pick tomorrow's date
   - Choose a time slot (e.g., 9:00 AM)
   - Fill in your name and email
   - Click "Confirm Booking"
   - You should see a success message!

### 2. Test Admin Features:
   - Go to "Admin" page
   - Click "Barber Info" tab
   - Click "Edit" button
   - Update the shop name
   - Click "Save"
   - Go back to "Home" to see the updated name

### 3. Add Portfolio Item:
   - Go to "Admin" page
   - Click "Portfolio" tab
   - Fill in:
     - Title: "Classic Fade"
     - Description: "Perfect fade haircut"
     - Image URL: "https://via.placeholder.com/400x300"
     - Order: 1
   - Click "Add Item"
   - Go to "Portfolio" page to see your new item

---

## Next Steps

- **Add Google Calendar Integration**: See README.md for Google Calendar API setup
- **Customize Styling**: Edit CSS files in `frontend/src/app/components/`
- **Add More Services**: Use the Admin panel to add more service types
- **Deploy**: See DEPLOYMENT.md for production deployment options

---

## Need Help?

- Check the main README.md for detailed documentation
- See API_DOCUMENTATION.md for API endpoint details
- Review DEPLOYMENT.md for deployment options
- Open an issue on GitHub if you encounter problems

---

**Happy Coding! 💈✂️**
