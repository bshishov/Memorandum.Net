using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors;

namespace Memorandum.Web.ViewModels
{
    public class DirectoryViewModel : ItemViewModel
    {
        private readonly IDirectoryItem _item;
        private List<DirectoryViewModel> _childDirs;
        private List<FileItemViewModel> _childFiles;

        public bool IsRoot => _item.IsRoot;
        public FileItemViewModel Index { get; }

        public override string ThumbnailTemplate => "Blocks/Thumbnails/dir";

        public List<DirectoryViewModel> ChildDirectories
        {
            get {
                return _childDirs ??
                       (_childDirs = _item.GetChildDirectories().Select(d => (DirectoryViewModel)EditorManager.GetDefaultEditor(d).ViewFactory.Create(d)).ToList());
            }
        }

        public List<FileItemViewModel> ChildFiles
        {
            get
            {
                return _childFiles ??
                       (_childFiles = _item.GetChildFiles().Select(d => (FileItemViewModel)EditorManager.GetDefaultEditor(d).ViewFactory.Create(d)).ToList());
            }
        }

        public DirectoryViewModel(IDirectoryItem node)
            : base(node)
        {
            _item = node;
            if(_item.Index != null)
                Index = new FileItemViewModel(_item.Index);
        }
    }
}