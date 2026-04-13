@echo off
echo Running Tests for Cutting Stock Problem Solver...
echo ===================================================

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK not found. Please install .NET 6.0 SDK.
    exit /b 1
)

REM Run tests
echo Running tests...
dotnet test tests/CuttingStockProblem.Tests.csproj
if errorlevel 1 (
    echo Error: Tests failed
    exit /b 1
)

echo Tests completed successfully!