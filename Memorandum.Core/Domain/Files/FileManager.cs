using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Files
{
    public static class FileManager
    {
        public static List<IDirectoryItem> MonitoredDirectories = new List<IDirectoryItem>();

        public static IItem Get(User user, string relativePath)
        {
            var fullPath = Path.Combine(user.Base.FileSystemPath, relativePath);
            
            var attr = File.GetAttributes(fullPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return GetDirectory(user, relativePath);
            }

            return GetFile(user, relativePath);
        }
    
        public static IDirectoryItem GetDirectory(User user, string relativePath)
        {
            relativePath = relativePath.Replace('\\', '/');
            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            if (string.IsNullOrEmpty(user.BaseDirectory))
                return null;

            var item = new DirectoryItem(user, relativePath);
            var attr = File.GetAttributes(item.FileSystemPath);
            
            if (!item.IsRoot && (attr.HasFlag(FileAttributes.Hidden) || attr.HasFlag(FileAttributes.System) || attr.HasFlag(FileAttributes.Temporary)))
                return null;

            if (item.Exists)
                return item;

            return null;
        }

        public static IFileItem GetFile(User user, string relativePath)
        {
            relativePath = relativePath.Replace('\\', '/');
            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            var item = new FileItem(user, relativePath);
            if (!item.Exists)
                return null;

            var attr = File.GetAttributes(item.FileSystemPath);
            if (attr.HasFlag(FileAttributes.Hidden) || attr.HasFlag(FileAttributes.System) || attr.HasFlag(FileAttributes.Temporary))
                return null;

            return item;
        }
    }
}
