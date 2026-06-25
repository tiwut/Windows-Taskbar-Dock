using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace WindowsTaskbarDock
{
    public class AppItem
    {
        public string Name { get; }
        public string FullPath { get; }
        public bool IsFolder { get; }
        public ImageSource Icon { get; }

        public AppItem(string fullPath)
        {
            FullPath = fullPath;
            IsFolder = Directory.Exists(fullPath);
            Name = IsFolder ? Path.GetFileName(fullPath) : Path.GetFileNameWithoutExtension(fullPath);
            Icon = NativeMethods.GetFileIcon(fullPath, IsFolder);
        }

        public static List<AppItem> ScanDirectory(string folderPath)
        {
            var items = new List<AppItem>();

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var dir in Directory.EnumerateDirectories(folderPath))
                {
                    var attr = File.GetAttributes(dir);
                    if ((attr & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        items.Add(new AppItem(dir));
                    }
                }

                foreach (var file in Directory.EnumerateFiles(folderPath))
                {
                    var attr = File.GetAttributes(file);
                    if ((attr & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        items.Add(new AppItem(file));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning directory {folderPath}: {ex.Message}");
            }

            return items
                .OrderByDescending(i => i.IsFolder)
                .ThenBy(i => i.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
    }
}
