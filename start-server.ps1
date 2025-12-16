# Start Backend Server
Write-Host "Starting ToDo App Backend Server..." -ForegroundColor Green
Set-Location ToDoApp.Server
dotnet run --launch-profile http-noproxy
Set-Location ..

