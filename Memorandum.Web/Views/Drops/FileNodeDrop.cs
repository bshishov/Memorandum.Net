using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    abstract class BaseFileNodeDrop : NodeDrop
    {
        protected BaseFileNode Node;
        public string Name { get { return Node.Name; } }
        public bool IsDirectory { get { return Node.IsDirectory; } }
        public string Path { get { return Node.Path; } }
        public DateTime LastModified { get { return Node.LastModified; } }

        protected BaseFileNodeDrop(BaseFileNode node) : base(node.NodeId)
        {
            Node = node;
        }
    }

    class FileNodeDrop : BaseFileNodeDrop
    {
        public long Size { get { return ((FileNode)Node).Size; } }
        public string Mime { get { return ((FileNode)Node).Mime; } }

        public FileNodeDrop(BaseFileNode node)
            : base(node)
        {

        }
    }

    class DirectoryNodeDrop : BaseFileNodeDrop
    {
        public DirectoryNodeDrop(BaseFileNode node)
            : base(node)
        {

        }
    }
}
