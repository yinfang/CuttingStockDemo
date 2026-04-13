@echo off
echo Starting Cutting Stock Problem Solver...
echo ======================================

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK not found. Please install .NET 6.0 SDK.
    exit /b 1
)

REM Run the application
echo Starting application...
dotnet run --project src/CuttingStockProblem.csproj
if errorlevel 1 (
    echo Error: Failed to start application
    exit /b 1
)

echo Application started successfully!