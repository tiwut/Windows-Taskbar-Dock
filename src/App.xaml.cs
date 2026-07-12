using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;

namespace WindowsTaskbarDock
{
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private static DateTime _lastDeactivatedTime = DateTime.MinValue;
        public static List<DockWindow> ActiveWindows { get; } = new List<DockWindow>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Wpf.Ui.Appearance.ApplicationThemeManager.ApplySystemTheme();

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = CreateGridIcon(),
                Visible = true,
                Text = "Windows Taskbar Dock"
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Open Dock Folder", null, (s, ev) => OpenDockFolder());
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (s, ev) => ExitApp());
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.MouseClick += (s, ev) =>
            {
                if (ev.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ToggleApp();
                }
            };

            Deactivated += (s, ev) =>
            {
                _lastDeactivatedTime = DateTime.UtcNow;
                CloseAllWindows();
            };
        }

        private Icon CreateGridIcon()
        {
            int size = 32;
            using (var bitmap = new Bitmap(size, size))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    
                    using (var brush = new SolidBrush(Color.White))
                    {
                        int padding = 3;
                        int gap = 3;
                        int gridAreaSize = size - (padding * 2);
                        int rectSize = (gridAreaSize - (gap * 2)) / 3;
                        
                        for (int r = 0; r < 3; r++)
                        {
                            for (int c = 0; c < 3; c++)
                            {
                                int x = padding + c * (rectSize + gap);
                                int y = padding + r * (rectSize + gap);
                                g.FillRectangle(brush, x, y, rectSize, rectSize);
                            }
                        }
                    }
                }
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }

        public static void ToggleApp()
        {
            if ((DateTime.UtcNow - _lastDeactivatedTime).TotalMilliseconds < 250)
            {
                return;
            }

            if (ActiveWindows.Count > 0)
            {
                CloseAllWindows();
            }
            else
            {
                string targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DockDesktop");
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                System.Windows.Point clickPoint = NativeMethods.GetMousePosition();
                var window = new DockWindow(targetPath, 0, null, clickPoint);
                window.Show();
                window.Activate();
                window.Focus();
            }
        }

        public static void CloseAllWindows()
        {
            if (ActiveWindows.Count == 0) return;

            var windowsToClose = ActiveWindows.ToList();
            ActiveWindows.Clear();

            foreach (var w in windowsToClose)
            {
                w.FadeOutAndClose();
            }
        }

        private void OpenDockFolder()
        {
            string targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DockDesktop");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            try
            {
                Process.Start("explorer.exe", targetPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to open folder: {ex.Message}");
            }
        }

        private void ExitApp()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
