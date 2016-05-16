using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            RelativePath = relativePath;
           _directoryInfo = new DirectoryInfo(Path.Combine(user.BaseDirectory, relativePath));
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
            _directoryInfo.Delete();
        }

        public IFileItem Index => FileManager.GetFile(Owner, Path.Combine(RelativePath, "Index.md"));

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

        public string GetHash()
        {
            var hash = Encoding.ASCII.GetBytes(Owner.Name + this.RelativePath);
            MD5 md5 = new MD5CryptoServiceProvider();
            var hashenc = md5.ComputeHash(hash);
            return hashenc.Aggregate("", (current, b) => current + b.ToString("x2"));
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
                retval.AddRange(dirs.Select(directoryInfo => FileManager.GetDirectory(Owner, Path.Combine(RelativePath, directoryInfo.Name))).Where(d => d != null));
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
                retval.AddRange(files.Select(fileInfo => FileManager.GetFile(Owner, Path.Combine(RelativePath, fileInfo.Name))).Where(f => f != null));
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