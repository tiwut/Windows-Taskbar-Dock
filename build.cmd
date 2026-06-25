@echo off
setlocal enabledelayedexpansion

set CLEAN_OPT=0
set BUILD_OPT=0
set RUN_OPT=0
set HAS_ARGS=0

:parse_args
if "%~1"=="" goto check_defaults
set HAS_ARGS=1
if /i "%~1"=="-clean" (
    set CLEAN_OPT=1
)
if /i "%~1"=="-build" (
    set BUILD_OPT=1
)
if /i "%~1"=="-run" (
    set RUN_OPT=1
)
shift
goto parse_args

:check_defaults
if %HAS_ARGS%==0 (
    set CLEAN_OPT=1
    set BUILD_OPT=1
)

set SCRIPT_DIR=%~dp0
set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%
set PROJECT_FILE=%SCRIPT_DIR%\src\WindowsTaskbarDock.csproj
set OUTPUT_DIR=%SCRIPT_DIR%\dist
set EXE_PATH=%OUTPUT_DIR%\WindowsTaskbarDock.exe

if %CLEAN_OPT%==1 (
    echo ==============================================
    echo Cleaning Project...
    echo ==============================================
    echo Stopping any running instances of Windows Taskbar Dock...
    taskkill /f /im WindowsTaskbarDock.exe >nul 2>&1
    if exist "%OUTPUT_DIR%" (
        echo Cleaning dist/ directory...
        rmdir /s /q "%OUTPUT_DIR%"
    )
    if exist "%SCRIPT_DIR%\src\bin" (
        echo Cleaning src/bin/ directory...
        rmdir /s /q "%SCRIPT_DIR%\src\bin"
    )
    if exist "%SCRIPT_DIR%\src\obj" (
        echo Cleaning src/obj/ directory...
        rmdir /s /q "%SCRIPT_DIR%\src\obj"
    )
    echo Cleanup completed successfully.
)

if %BUILD_OPT%==1 (
    echo ==============================================
    echo Building Windows Taskbar Dock...
    echo ==============================================
    if not %CLEAN_OPT%==1 (
        echo Stopping any running instances of Windows Taskbar Dock...
        taskkill /f /im WindowsTaskbarDock.exe >nul 2>&1
    )
    echo Compiling standalone single-file executable...
    dotnet publish "%PROJECT_FILE%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o "%OUTPUT_DIR%"
    if !ERRORLEVEL! EQU 0 (
        echo.
        echo ==============================================
        echo BUILD SUCCESSFUL!
        echo Executable generated at: %EXE_PATH%
        echo ==============================================
    ) else (
        echo Build failed with exit code !ERRORLEVEL!.
        exit /b !ERRORLEVEL!
    )
)

if %RUN_OPT%==1 (
    echo ==============================================
    echo Running Windows Taskbar Dock...
    echo ==============================================
    if exist "%EXE_PATH%" (
        start "" "%EXE_PATH%"
        echo Application started.
    ) else (
        echo Error: Executable not found at %EXE_PATH%. Please build the project first.
        exit /b 1
    )
)
