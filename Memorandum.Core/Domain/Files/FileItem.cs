using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Memorandum.Core.Domain.Users;
using MimeTypes;

namespace Memorandum.Core.Domain.Files
{
    public class FileItem : IFileItem
    {
        private readonly FileInfo _info;

        public string FileSystemPath => _info.FullName;
        public bool IsLink { get; }
        public string Name => _info.Name;
        public DateTime Created => _info.CreationTime;
        public DateTime Modified => _info.LastWriteTime;
        public bool IsDirectory => false;
        public string RelativePath { get; private set; }
        public User Owner { get; }
        public bool Exists => _info.Exists;

        public FileItem(User user, string relativePath)
        {
            Owner = user;
            RelativePath = relativePath;
            _info = new FileInfo(Path.Combine(user.BaseDirectory, relativePath));
        }

        public void Rename(string newName)
        {
            if(_info.DirectoryName == null) 
                throw new InvalidOperationException("DirectoryName == null");
            RelativePath = Path.Combine(Path.GetDirectoryName(RelativePath), newName);
            _info.MoveTo(Path.Combine(Path.GetFullPath(_info.DirectoryName), newName));
        }

        public void MoveTo(IDirectoryItem target)
        {
            if (_info.DirectoryName == null)
                throw new InvalidOperationException("DirectoryName == null");
            throw new NotImplementedException();
            _info.MoveTo(Path.GetFullPath(_info.DirectoryName));
        }

        public void CopyTo(IDirectoryItem target)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            _info.Delete();
        }

        public string NameWithoutExtension { get; }
        public string Extension => _info.Extension;
        public string Mime => MimeTypeMap.GetMimeType(_info.Extension);
        public long Size => _info.Length;

        public Stream GetStream(FileMode mode = FileMode.Open)
        {
            return File.Open(_info.FullName, mode);
        }

        public IDirectoryItem GetParent()
        {
            return FileManager.GetDirectory(Owner, Path.GetDirectoryName(RelativePath));
        }

        public string GetHash()
        {
            var hash = Encoding.ASCII.GetBytes(Owner.Name + this.RelativePath);
            MD5 md5 = new MD5CryptoServiceProvider();
            var hashenc = md5.ComputeHash(hash);
            return hashenc.Aggregate("", (current, b) => current + b.ToString("x2"));
        }

        public static IFileItem Create(User owner, string relativePath)
        {
            var item = new FileItem(owner, relativePath);
            using (File.Create(item.FileSystemPath)) { }
            return item;
        }
    }
}