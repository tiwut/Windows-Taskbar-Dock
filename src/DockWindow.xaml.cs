using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WindowsTaskbarDock
{
    public partial class DockWindow : Wpf.Ui.Controls.FluentWindow
    {
        public string FolderPath { get; }
        public int Level { get; }
        public DockWindow? ParentWindow { get; }
        private System.Windows.Point? _clickPoint;

        public DockWindow(string folderPath, int level, DockWindow? parentWindow, System.Windows.Point? clickPoint = null)
        {
            InitializeComponent();
            FolderPath = folderPath;
            Level = level;
            ParentWindow = parentWindow;
            _clickPoint = clickPoint;

            App.ActiveWindows.Add(this);
            this.Deactivated += DockWindow_Deactivated;

            WindowRoot.Opacity = 0;
        }

        private void DockWindow_Deactivated(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!App.ActiveWindows.Any(w => w.IsActive))
                {
                    App.CloseAllWindows();
                }
            }));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            PositionWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NativeMethods.SetWindowStyles(this);

            LoadItems();

            PlayEntryAnimation();
        }

        private void LoadItems()
        {
            var items = AppItem.ScanDirectory(FolderPath);
            ItemsList.ItemsSource = items;

            if (items.Count == 0)
            {
                EmptyLabel.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void PositionWindow()
        {
            if (Level == 0 && _clickPoint.HasValue)
            {
                var clickPt = _clickPoint.Value;
                
                var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)clickPt.X, (int)clickPt.Y));
                var workArea = screen.WorkingArea;
                var bounds = screen.Bounds;

                double x = 0;
                double y = 0;

                if (workArea.Bottom < bounds.Bottom)
                {
                    x = clickPt.X - (this.Width / 2);
                    y = workArea.Bottom - this.Height - 12;
                }
                else if (workArea.Top > bounds.Top)
                {
                    x = clickPt.X - (this.Width / 2);
                    y = workArea.Top + 12;
                }
                else if (workArea.Left > bounds.Left)
                {
                    x = workArea.Left + 12;
                    y = clickPt.Y - (this.Height / 2);
                }
                else if (workArea.Right < bounds.Right)
                {
                    x = workArea.Right - this.Width - 12;
                    y = clickPt.Y - (this.Height / 2);
                }
                else
                {
                    x = clickPt.X - (this.Width / 2);
                    y = workArea.Bottom - this.Height - 12;
                }

                if (x < workArea.Left + 12) x = workArea.Left + 12;
                if (x + this.Width > workArea.Right - 12) x = workArea.Right - this.Width - 12;
                if (y < workArea.Top + 12) y = workArea.Top + 12;
                if (y + this.Height > workArea.Bottom - 12) y = workArea.Bottom - this.Height - 12;

                this.Left = x;
                this.Top = y;
            }
            else if (ParentWindow != null)
            {
                var screen = System.Windows.Forms.Screen.FromRectangle(
                    new System.Drawing.Rectangle((int)ParentWindow.Left, (int)ParentWindow.Top, (int)ParentWindow.Width, (int)ParentWindow.Height));
                var workArea = screen.WorkingArea;

                double x = ParentWindow.Left + ParentWindow.Width + 12;
                double y = ParentWindow.Top;

                if (x + this.Width > workArea.Right - 12)
                {
                    x = ParentWindow.Left - this.Width - 12;
                }

                if (y + this.Height > workArea.Bottom - 12)
                {
                    y = workArea.Bottom - this.Height - 12;
                }
                if (y < workArea.Top + 12)
                {
                    y = workArea.Top + 12;
                }

                this.Left = x;
                this.Top = y;
            }
        }

        private void PlayEntryAnimation()
        {
            var sb = new Storyboard();

            var fadeAnim = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.24))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(fadeAnim, WindowRoot);
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fadeAnim);

            var slideAnim = new DoubleAnimation(20.0, 0.0, TimeSpan.FromSeconds(0.26))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.15 }
            };
            Storyboard.SetTarget(slideAnim, WindowTranslate);
            Storyboard.SetTargetProperty(slideAnim, new PropertyPath(TranslateTransform.YProperty));
            sb.Children.Add(slideAnim);

            sb.Begin();
        }

        public void FadeOutAndClose()
        {
            var sb = new Storyboard();

            var fadeAnim = new DoubleAnimation(WindowRoot.Opacity, 0.0, TimeSpan.FromSeconds(0.18))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(fadeAnim, WindowRoot);
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fadeAnim);

            var slideAnim = new DoubleAnimation(WindowTranslate.Y, 15.0, TimeSpan.FromSeconds(0.20))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(slideAnim, WindowTranslate);
            Storyboard.SetTargetProperty(slideAnim, new PropertyPath(TranslateTransform.YProperty));
            sb.Children.Add(slideAnim);

            sb.Completed += (s, e) =>
            {
                try
                {
                    this.Close();
                }
                catch
                {
                }
            };
            sb.Begin();
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is AppItem item)
            {
                if (item.IsFolder)
                {
                    var windowsToClose = App.ActiveWindows.Where(w => w.Level > this.Level).ToList();
                    foreach (var w in windowsToClose)
                    {
                        App.ActiveWindows.Remove(w);
                        w.FadeOutAndClose();
                    }

                    var childWindow = new DockWindow(item.FullPath, this.Level + 1, this);
                    childWindow.Show();
                    childWindow.Activate();
                    childWindow.Focus();
                }
                else
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = item.FullPath,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Could not open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    App.CloseAllWindows();
                }
            }
        }
    }
}
