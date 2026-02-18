@echo off
echo ===============================================
echo   Starting Filament Frontend for SDI_FINANCE
echo ===============================================
echo.

cd filaments-app

echo [1/4] Checking dependencies...
if not exist "vendor\" (
    echo Installing Composer dependencies...
    call composer install
)

if not exist "node_modules\" (
    echo Installing NPM dependencies...
    call npm install
)

echo.
echo [2/4] Building assets...
call npm run build

echo.
echo [3/4] Clearing cache...
call php artisan config:clear
call php artisan route:clear
call php artisan view:clear

echo.
echo [4/4] Starting Laravel server...
echo.
echo ===============================================
echo   Filament Frontend is starting...
echo   Access at: http://localhost:8000/admin
echo ===============================================
echo.

start /B php artisan serve

echo.
echo Server is running. Press Ctrl+C to stop.
pause
