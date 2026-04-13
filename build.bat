@echo off
echo Building Cutting Stock Problem Solver...
echo ==========================================

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK not found. Please install .NET 6.0 SDK.
    exit /b 1
)

REM Restore packages
echo Restoring packages...
dotnet restore
if errorlevel 1 (
    echo Error: Failed to restore packages
    exit /b 1
)

REM Build the project
echo Building project...
dotnet build
if errorlevel 1 (
    echo Error: Build failed
    exit /b 1
)

echo Build completed successfully!