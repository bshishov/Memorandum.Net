using System;
using Memorandum.Core.Domain;
using Memorandum.Web.Views.Providers;

namespace Memorandum.Web.Views.Drops
{
    internal abstract class BaseFileNodeDrop : NodeDrop
    {
        protected BaseFileNode Node;

        protected BaseFileNodeDrop(BaseFileNode node) : base(node)
        {
            Node = node;
        }

        public string Name => Node.Name;

        public bool IsDirectory => Node.IsDirectory;

        public string Path => Node.Path;

        public DateTime LastModified => Node.LastModified;
    }

    internal class FileNodeDrop : BaseFileNodeDrop
    {
        public FileNodeDrop(BaseFileNode node)
            : base(node)
        {
        }

        public long Size => ((FileNode) Node).Size;

        public string Mime => ((FileNode) Node).Mime;

        public string Extension => FileProvider.GetKnownExtension((FileNode) Node);
    }

    internal class DirectoryNodeDrop : BaseFileNodeDrop
    {
        public DirectoryNodeDrop(BaseFileNode node)
            : base(node)
        {
        }
    }
}