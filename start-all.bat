@echo off
echo Starting Treasure Hunt Application...
echo.
echo Current directory: %cd%
echo.

echo Starting Backend API Server...
start "Backend API" cmd /k "cd /d "%~dp0Backend" && dotnet run"

echo Waiting for Backend to start...
timeout /t 5 /nobreak > nul

echo Starting Frontend React App...
start "Frontend React" cmd /k "cd /d "%~dp0Frontend" && npm start"

echo.
echo Both services are starting...
echo Backend API: https://localhost:7000
echo Frontend React: http://localhost:3000
echo.
echo Press any key to exit...
pause > nul 