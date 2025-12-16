@echo off
echo Starting ToDo App (Backend + Frontend)...
start "ToDo App Backend" cmd /k "cd /d %~dp0ToDoApp.Server && dotnet run --launch-profile http-noproxy"
timeout /t 3 /nobreak >nul
start "ToDo App Frontend" cmd /k "cd /d %~dp0todoapp.client && npm run dev"
echo.
echo Backend and Frontend started in separate windows.
echo Backend: http://localhost:5180
echo Frontend: http://localhost:5173
pause

