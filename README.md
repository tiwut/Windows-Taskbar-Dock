# Windows Taskbar Dock

A premium, high-performance native Windows system tray companion inspired by macOS folder status docks. It runs quietly in the notification area, providing fluid, multi-column navigation for your files and directories with GPU-accelerated Acrylic blur backdrops, native rounded corners, and native drop shadows.

---

## Features

*   **Native System Tray Integration**: Operates inside the Windows notification area using a custom-rendered grid status icon.
*   **WPF-UI Integration**: Utilizes the standard WPF-UI library to deliver pixel-perfect Windows 11 fluent design.
*   **Native Backdrops**: Features native Windows 11 Acrylic backdrops and automatic system theme (Light/Dark mode) switching.
*   **60+ FPS Content Animations**: Animates content opacity and transition transforms directly, keeping the UI fully hardware-accelerated.
*   **Hierarchical Navigation**: Clicking a folder slides open a secondary navigation column side-by-side.
*   **Folders-First Sorting**: Group directories at the top-left, followed by files in alphabetical order.
*   **Hidden File Extensions**: Hides file endings (like `.lnk`, `.txt`, `.py`) for a clean interface.
*   **Auto-Dismissal**: Toggling outside any open dock panel closes the entire flyout structure immediately.

---

## Project Structure

*   [src/](src/) - Source code directory containing:
    *   [WindowsTaskbarDock.csproj](src/WindowsTaskbarDock.csproj) - Project dependencies (now including WPF-UI) and publish settings.
    *   [App.xaml](src/App.xaml) & [App.xaml.cs](src/App.xaml.cs) - Application lifecycle, theme initialization, and system tray icon setup.
    *   [DockWindow.xaml](src/DockWindow.xaml) & [DockWindow.xaml.cs](src/DockWindow.xaml.cs) - Layout, positioning, and animations using WPF-UI `FluentWindow`.
    *   [AppItem.cs](src/AppItem.cs) - File scanner, grouping, and name formatting logic.
    *   [NativeMethods.cs](src/NativeMethods.cs) - Win32 API declarations for shell icons and mouse position.
*   [build.cmd](build.cmd) - Automated command script to clean, build, or run the project.

---

## Requirements

*   **To Run**: Windows 11 or Windows 10 (Build 1803+).
*   **To Build**: .NET 8.0 SDK.

---

## How to Build

1. Open a Command Prompt (cmd) or PowerShell in the project directory.
2. Run `build.cmd`. By default, it cleans old outputs and compiles the project:
   ```cmd
   .\build.cmd
   ```

   **Optional parameters (can be run separately or combined):**
   *   **Clean Only**: Wipes the `dist/` directory and C# `bin`/`obj` build cache folders:
       ```cmd
       .\build.cmd -clean
       ```
   *   **Build Only**: Publishes the standalone EXE without deleting build cache files:
       ```cmd
       .\build.cmd -build
       ```
   *   **Run Only**: Launches the compiled application from the `dist/` directory:
       ```cmd
       .\build.cmd -run
       ```
   *   **Combine Options**: For example, to build and run sequentially:
       ```cmd
       .\build.cmd -build -run
       ```
3. The standalone, single-file executable is generated at:
   `dist/WindowsTaskbarDock.exe`

---

## How to Use

1. Run `dist/WindowsTaskbarDock.exe` (or run `.\build.cmd -run`).
2. A grid icon (`▣`) will appear in your system tray (bottom-right near the clock).
3. The application will automatically create a folder named `DockDesktop` in your User Profile folder:
   `C:\Users\<YourUsername>\DockDesktop`
4. Place folders, application shortcuts, web links, or standard documents inside the `DockDesktop` directory.
5. Click the tray icon to navigate! Right-click the tray icon and select **Exit** to close.
