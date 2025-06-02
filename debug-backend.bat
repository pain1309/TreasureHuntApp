@echo off
echo =================================
echo Treasure Hunt Backend Debug
echo =================================
echo.

echo Checking if .NET is installed...
dotnet --version
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8.0 SDK
    pause
    exit /b 1
)

echo.
echo Navigating to Backend directory...
cd /d "%~dp0Backend"

echo.
echo Current directory: %cd%

echo.
echo Building project...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo Starting backend server...
echo Backend will be available at:
echo   - http://localhost:5000/api
echo   - https://localhost:7001/api
echo   - Swagger: http://localhost:5000/swagger
echo.
echo Press Ctrl+C to stop the server
echo.

dotnet run 