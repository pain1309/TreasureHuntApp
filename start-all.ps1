Write-Host "Starting Treasure Hunt Application..." -ForegroundColor Green
Write-Host ""
Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
Write-Host ""

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Starting Backend API Server..." -ForegroundColor Cyan
$backendPath = Join-Path $scriptDir "Backend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; dotnet run" -WindowStyle Normal

Write-Host "Waiting for Backend to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Starting Frontend React App..." -ForegroundColor Cyan  
$frontendPath = Join-Path $scriptDir "Frontend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; npm start" -WindowStyle Normal

Write-Host ""
Write-Host "Both services are starting..." -ForegroundColor Green
Write-Host "Backend API: https://localhost:7000" -ForegroundColor White
Write-Host "Frontend React: http://localhost:3000" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 