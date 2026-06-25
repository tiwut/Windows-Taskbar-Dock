# Windows Taskbar Dock

A native Windows status dock running in the system tray, providing clean folder navigation flyouts inspired by macOS. It displays files and folders with native rounded corners, system drop shadows, and hardware-accelerated Acrylic blur backdrops.

## Features

- **System Tray Integration**: Quietly runs in the tray with a grid icon. Left-click to open.
- **Acrylic Backdrops & Windows 11 Styling**: Native rounded corners, drop shadows, and blurry Acrylic surfaces.
- **Hierarchical Folders**: Grouped folders are listed first. Clicking folders slides open nested panels to the side.
- **Focus Auto-Dismiss**: Clicking anywhere else immediately closes all panels.
- **Clean File Names**: Hides extensions (like `.lnk`, `.txt`, `.url`, `.py`) and fetches high-res native icons.
- **Standalone EXE**: Ready to run as a single, self-contained executable.

## Prerequisites

- Windows 11 or Windows 10 (Build 1803+)
- .NET 8.0 SDK (only needed to compile from source)

## How to Build

1. Open PowerShell in the project directory.
2. Run the build script:
   ```powershell
   .\build.ps1
   ```
3. Find your standalone executable in: `dist/WindowsTaskbarDock.exe`

## How to Use

1. Run the compiled `WindowsTaskbarDock.exe`.
2. The application will monitor a folder named `DockDesktop` inside your User Profile (e.g. `C:\Users\YourName\DockDesktop`). If it doesn't exist, the app creates it on startup.
3. Place files, directories, shortcuts (`.lnk`), or internet shortcuts (`.url`) inside `DockDesktop`.
4. Click the tray grid icon (`▣`) to view and navigate your files!
5. To exit, right-click the tray icon and select **Exit**.

## Project Structure

- [src/](src/) - Main C# WPF source code.
  - [App.xaml.cs](src/App.xaml.cs) - Application lifecycle and tray icon.
  - [DockWindow.xaml.cs](src/DockWindow.xaml.cs) - Layout, positioning, and animation logic.
  - [AppItem.cs](src/AppItem.cs) - Files and folders scanning/sorting.
  - [NativeMethods.cs](src/NativeMethods.cs) - Win32/DWM native properties.
- [build.ps1](build.ps1) - Windows build script.
