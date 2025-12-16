# ToDo App - Startup Scripts

This project includes scripts to easily start the backend server and frontend client.

## Quick Start

### Option 1: NPM Scripts (Cross-Platform - Recommended)
First, install dependencies:
```bash
npm install
```

Then start both servers:
```bash
npm run dev
```

Or start individually:
- **Backend only:** `npm run dev:server`
- **Frontend only:** `npm run dev:client`

### Option 2: PowerShell Scripts (Windows)
Run the script to start both backend and frontend in separate windows:
- **Start Both:** `.\start-all.ps1`
- **Backend Only:** `.\start-server.ps1`
- **Frontend Only:** `.\start-client.ps1`

### Option 3: Batch Scripts (Windows Command Prompt)
- **Start Both:** `start-all.bat`
- **Backend Only:** `start-server.bat`
- **Frontend Only:** `start-client.bat`

### Option 4: Manual
- **Backend:** `cd ToDoApp.Server && dotnet run`
- **Frontend:** `cd todoapp.client && npm run dev`

## URLs

Once started:
- **Backend API:** http://localhost:5180
- **Frontend App:** http://localhost:5173
- **Swagger UI:** http://localhost:5180/swagger

## Default Admin Account

When the backend starts for the first time, an admin account is automatically created:
- **Username:** `admin`
- **Password:** `admin123`

## Notes

- Make sure you have Node.js and .NET 8 SDK installed
- The backend will automatically create the SQLite database on first run
- If you get connection errors, ensure the backend is running before starting the frontend
- For NPM scripts, you'll need to install `concurrently` package (run `npm install` in the root directory)
