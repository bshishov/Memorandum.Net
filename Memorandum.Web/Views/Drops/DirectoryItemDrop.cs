using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.Views.Drops
{
    public class DirectoryItemDrop : ItemDrop
    {
        private readonly IDirectoryItem _item;
        private List<DirectoryItemDrop> _childDirs;
        private List<FileItemDrop> _childFiles;

        public bool IsRoot => _item.IsRoot;
        public FileItemDrop Index { get; }

        public List<DirectoryItemDrop> ChildDirectories
        {
            get {
                return _childDirs ??
                       (_childDirs = _item.GetChildDirectories().Select(d => new DirectoryItemDrop(d)).ToList());
            }
        }

        public List<FileItemDrop> ChildFiles
        {
            get
            {
                return _childFiles ??
                       (_childFiles = _item.GetChildFiles().Select(d => new FileItemDrop(d)).ToList());
            }
        }

        public DirectoryItemDrop(IDirectoryItem node)
            : base(node)
        {
            _item = node;
            if(_item.Index != null)
                Index = new FileItemDrop(_item.Index);
        }
    }
}