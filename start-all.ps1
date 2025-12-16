# Start Both Backend and Frontend
Write-Host "Starting ToDo App (Backend + Frontend)..." -ForegroundColor Cyan

# Start backend in a new window (without SPA proxy)
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\ToDoApp.Server'; dotnet run --launch-profile http-noproxy" -WindowStyle Normal

# Wait a moment for backend to start
Start-Sleep -Seconds 3

# Start frontend in a new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\todoapp.client'; npm run dev" -WindowStyle Normal

Write-Host "Backend and Frontend started in separate windows." -ForegroundColor Green
Write-Host "Backend: http://localhost:5180" -ForegroundColor Yellow
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Yellow

