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
            //this.Path = this.Path.Replace("\\", "/");
            Name = System.IO.Path.GetFileName(path);
        }

        public override User User { get; set; }

        public override NodeIdentifier NodeId => new NodeIdentifier("file", Path);
    }

    public class FileNode : BaseFileNode
    {
        public override bool IsDirectory => false;
        public readonly long Size;
        public string Mime;

        public override DateTime LastModified { get; }
        
        public FileNode(string path) : base(path)
        {
            var info = new FileInfo(path);
            Size = info.Length;
            LastModified = info.LastWriteTime;
            Mime = MimeTypeMap.GetMimeType(System.IO.Path.GetExtension(path));
        }
    }

    public class DirectoryNode : BaseFileNode
    {
        public override DateTime LastModified { get; }

        public override bool IsDirectory => true;

        public DirectoryNode(string path) : base(path)
        {
            LastModified = Directory.GetLastWriteTime(path);
        }

        public IEnumerable<BaseFileNode> GetChild()
        {
            var dirs = Directory.GetDirectories(Path).Select(p => new DirectoryNode(p)).Cast<BaseFileNode>().ToList();
            dirs.AddRange(Directory.GetFiles(Path).Select(p => new FileNode(p)));
            return dirs;
        }

        public void PerformOnChild(Action<BaseFileNode> actionToPerform, bool recursive = false)
        {
            if (actionToPerform == null)
                return;

            var child = GetChild();
            foreach (var c in child)
            {
                if (recursive)
                    (c as DirectoryNode)?.PerformOnChild(actionToPerform, true);

                actionToPerform(c);
            }
        }
    }
}