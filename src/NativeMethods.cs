using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsTaskbarDock
{
    internal static class NativeMethods
    {
        #region Window Style API

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        internal static void SetWindowStyles(Window window)
        {
            var helper = new WindowInteropHelper(window);
            IntPtr hwnd = helper.Handle;

            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
        }

        #endregion


        #region Shell Icon API

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        internal static ImageSource GetFileIcon(string path, bool isFolder)
        {
            var shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_LARGEICON;

            if (isFolder)
            {
                SHGetFileInfo(path, 0x10, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags | SHGFI_USEFILEATTRIBUTES);
            }
            else
            {
                SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            }

            if (shinfo.hIcon == IntPtr.Zero)
            {
                return new BitmapImage();
            }

            try
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    shinfo.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                
                imageSource.Freeze();
                return imageSource;
            }
            finally
            {
                DestroyIcon(shinfo.hIcon);
            }
        }

        #endregion

        #region Cursor position API

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        internal static System.Windows.Point GetMousePosition()
        {
            GetCursorPos(out POINT p);
            return new System.Windows.Point(p.X, p.Y);
        }

        #endregion
    }
}
