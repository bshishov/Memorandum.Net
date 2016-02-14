using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeTypes;

namespace Memorandum.Core.Domain
{
    public abstract class BaseFileNode : Node
    {
        public string Path;
        public string Name;
        public abstract bool IsDirectory { get; }
        public abstract DateTime LastModified { get; }

        protected BaseFileNode(string path)
        {
            this.Path = path;
            this.Path = this.Path.Replace("\\", "/");
            Name = System.IO.Path.GetFileName(path);
        }

        public override User User { get; set; }

        public override NodeIdentifier NodeId
        {
            get { return new NodeIdentifier("file", Path); }
        }
    }

    public class FileNode : BaseFileNode
    {
        public override bool IsDirectory { get { return false; } }
        public readonly long Size;
        public string Mime;

        private readonly DateTime _lastModified;

        public override DateTime LastModified
        {
            get { return _lastModified; }
        }


        public FileNode(string path) : base(path)
        {
            var info = new FileInfo(path);
            Size = info.Length;
            _lastModified = info.LastWriteTime;
            Mime = MimeTypeMap.GetMimeType(System.IO.Path.GetExtension(path));
        }
    }

    public class DirectoryNode : BaseFileNode
    {
        private readonly DateTime _lastModified;

        public override DateTime LastModified
        {
            get { return _lastModified; }
        }

        public override bool IsDirectory { get { return true; } }

        public DirectoryNode(string path) : base(path)
        {
            _lastModified = Directory.GetLastWriteTime(path);
        }

        public IEnumerable<BaseFileNode> GetChild()
        {
            var dirs = Directory.GetDirectories(Path).Select(p => new DirectoryNode(p)).Cast<BaseFileNode>().ToList();
            dirs.AddRange(Directory.GetFiles(Path).Select(p => new FileNode(p)));
            return dirs;
        }
    }
}