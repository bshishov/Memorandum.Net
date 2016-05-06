using System.Collections.Generic;
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
            return new DirectoryItem(user, relativePath);
        }

        public static IFileItem GetFile(User user, string relativePath)
        {
            relativePath = relativePath.Replace('\\', '/');
            var item = new FileItem(user, relativePath);
            if (File.Exists(item.FileSystemPath))
                return item;

            return null;
        }
    }
}
