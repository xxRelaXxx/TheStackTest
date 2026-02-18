@echo off
echo ========================================
echo  CrossDocker - Finance API
echo  Gestionale Finanziario Backend
echo ========================================
echo.

cd Finance-api

echo Checking database...
if not exist "finance.db" (
    echo Creating database...
    dotnet ef database update
)

echo.
echo Starting API server...
echo API will be available at:
echo   - HTTPS: https://localhost:7241
echo   - HTTP:  http://localhost:5241
echo   - Swagger: https://localhost:7241/swagger
echo.
echo Press Ctrl+C to stop the server
echo.

dotnet run

pause
