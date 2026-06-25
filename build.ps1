$ErrorActionPreference = "Stop"

Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "Building Windows Taskbar Dock..." -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ScriptDir "src\WindowsTaskbarDock.csproj"
$OutputDir = Join-Path $ScriptDir "dist"

Write-Host "Stopping any running instances of Windows Taskbar Dock..." -ForegroundColor Gray
Stop-Process -Name "WindowsTaskbarDock" -Force -ErrorAction SilentlyContinue

if (Test-Path $OutputDir) {
    Write-Host "Cleaning existing dist directory..." -ForegroundColor Gray
    Remove-Item $OutputDir -Recurse -Force
}

Write-Host "Compiling standalone single-file executable..." -ForegroundColor Yellow
dotnet publish $ProjectFile `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishReadyToRun=true `
    -o $OutputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "==============================================" -ForegroundColor Green
    Write-Host "BUILD SUCCESSFUL!" -ForegroundColor Green
    Write-Host "Executable generated at: $(Join-Path $OutputDir 'WindowsTaskbarDock.exe')" -ForegroundColor Green
    Write-Host "==============================================" -ForegroundColor Green
    Write-Host "Instructions:" -ForegroundColor Gray
    Write-Host "1. Double-click the generated 'WindowsTaskbarDock.exe' to run the app." -ForegroundColor Gray
    Write-Host "2. It will create a folder named 'DockDesktop' in your User Profile folder (e.g. C:\Users\<Username>\DockDesktop)." -ForegroundColor Gray
    Write-Host "3. Place shortcuts, files, and folders in 'DockDesktop' and click the tray icon to navigate." -ForegroundColor Gray
} else {
    Write-Error "Build failed with exit code $LASTEXITCODE."
}
