@echo off
setlocal EnableDelayedExpansion

:: ============================================================================
::  WHEEL WIZARD BUILD SCRIPT
:: ============================================================================
cls
echo ================================================================
echo                   WHEEL WIZARD BUILD SCRIPT
echo ================================================================
echo.

:: ---------------------------------------------------------------------------
:: [1/2] Publishing .NET Project
:: ---------------------------------------------------------------------------
echo [1/2] Publishing .NET project...
echo ---------------------------------------------------------------
echo Running:
echo dotnet publish -r linux-x64 -c Release ^ 
  /p:PublishSingleFile=true ^ 
  /p:IncludeAllContentForSelfExtract=true ^ 
  /p:IncludeNativeLibrariesForSelfExtract=true ^ 
  /p:EnableCompressionInSingleFile=true --self-contained true
echo.
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true --self-contained true
if errorlevel 1 (
    echo.
    echo [ERROR] Dotnet publish failed with error code %errorlevel%.
    goto end
)
echo.
echo [SUCCESS] Dotnet publish completed successfully.
echo.

:: ---------------------------------------------------------------------------
:: [2/2] Setting Executable Permissions
:: ---------------------------------------------------------------------------
echo [2/2] Setting executable permissions on the published file...
echo ---------------------------------------------------------------
set "PUBLISH_DIR=WheelWizard\bin\Release\net7.0\linux-x64\publish"
set "PUBLISH_EXE=%PUBLISH_DIR%\WheelWizard"

:: Check if chmod command is available (usually not present on default Windows)
where chmod >nul 2>&1
if errorlevel 1 (
    echo [WARNING] 'chmod' command not found. Skipping permission change.
) else (
    echo Running:
    echo chmod +x "%PUBLISH_EXE%"
    echo.
    chmod +x "%PUBLISH_EXE%"
    if errorlevel 1 (
        echo.
        echo [ERROR] chmod command failed with error code %errorlevel%.
        goto end
    )
    echo.
    echo [SUCCESS] chmod command completed successfully.
)
echo.

:: ============================================================================
::                           BUILD COMPLETED
:: ============================================================================
echo ================================================================
echo                      BUILD SUCCESSFUL!
echo ================================================================
echo.
:end
pause
