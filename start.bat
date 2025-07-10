@echo off
echo Starting TrendLens Sales Data Analyzer...
echo.

echo [1/4] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo [2/4] Building solution...
dotnet build --configuration Release --no-restore
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo [3/4] Running tests...
dotnet test --no-build --configuration Release --verbosity minimal
if %errorlevel% neq 0 (
    echo WARNING: Some tests failed
)

echo [4/4] Starting web application...
echo.
echo TrendLens will be available at:
echo - Web UI: https://localhost:5001
echo - API: https://localhost:5001/swagger
echo.
echo Press Ctrl+C to stop the application
echo.

cd src\TrendLens.Web
dotnet run --configuration Release --no-build
