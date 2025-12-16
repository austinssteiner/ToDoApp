@echo off
echo Starting ToDo App Backend Server...
cd ToDoApp.Server
dotnet run --launch-profile http-noproxy
cd ..

