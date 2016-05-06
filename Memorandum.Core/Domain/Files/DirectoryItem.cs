using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Files
{
    public class DirectoryItem : IDirectoryItem
    {
        private readonly DirectoryInfo _directoryInfo;

        public bool IsRoot => RelativePath.Equals("/") || RelativePath.Equals(String.Empty);
        public string FileSystemPath => _directoryInfo.FullName;
        public bool IsLink => false;
        public string Name => _directoryInfo.Name;
        public DateTime Created => _directoryInfo.CreationTime;
        public DateTime Modified => _directoryInfo.LastWriteTime;
        public bool IsDirectory => true;
        public string RelativePath { get; private set; }
        public User Owner { get; }
        public bool Exists => _directoryInfo.Exists;

        public DirectoryItem(User user, string relativePath)
        {
            Owner = user;
            relativePath = relativePath.Replace('\\', '/');
            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);
            RelativePath = relativePath;
           _directoryInfo = new DirectoryInfo(Path.Combine(user.BaseDirectory, relativePath));

            Index = FileManager.GetFile(user, Path.Combine(relativePath, "Index.md"));
        }

        public void Rename(string newName)
        {
            RelativePath = Path.Combine(Path.GetDirectoryName(RelativePath), newName);
            _directoryInfo.MoveTo(Path.Combine(_directoryInfo.Parent.FullName, newName));
        }

        public void MoveTo(IDirectoryItem target)
        {
            throw new NotImplementedException();
            _directoryInfo.MoveTo(target.FileSystemPath);
        }

        public void CopyTo(IDirectoryItem target)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IFileItem Index { get; }

        public void PerformOnchild(Action<IItem> action, bool recursive = false)
        {
            throw new NotImplementedException();
        }

        public IDirectoryItem GetParent()
        {
            if (IsRoot)
                return null;
            
            if (_directoryInfo.Parent != null)
            {
                return FileManager.GetDirectory(Owner, Path.GetDirectoryName(RelativePath));
            }

            return null;
        }

        public IEnumerable<IItem> GetChild()
        {
            var child = new List<IItem>();
            child.AddRange(GetChildDirectories());
            child.AddRange(GetChildFiles());
            return child;
        }

        public IEnumerable<IDirectoryItem> GetChildDirectories()
        {
            var retval = new List<IDirectoryItem>();
            try
            {
                var dirs = _directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                retval.AddRange(dirs.Select(directoryInfo => FileManager.GetDirectory(Owner, Path.Combine(RelativePath, directoryInfo.Name))));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return retval;
        }

        public IEnumerable<IFileItem> GetChildFiles()
        {
            var retval = new List<IFileItem>();
            try
            {
                var files = _directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                retval.AddRange(files.Select(fileInfo => FileManager.GetFile(Owner, Path.Combine(RelativePath, fileInfo.Name))));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return retval;
        }

        public static DirectoryItem Create(User user, string relativePath)
        {
            var dir = new DirectoryItem(user, relativePath);
            Directory.CreateDirectory(dir.FileSystemPath);
            return dir;
        }
    }
}