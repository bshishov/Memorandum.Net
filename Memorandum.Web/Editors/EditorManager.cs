using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Creators;

namespace Memorandum.Web.Editors
{
    static class EditorManager
    {
        private static readonly List<Editor<IFileItem>> FileEditors;
        private static readonly List<Editor<IDirectoryItem>> DirectoryEditors;

        static EditorManager()
        {
            FileEditors = new List<Editor<IFileItem>>
            {
                new FileDefaultEditor(),
                new FileCodeEditor(),
                new FileImageEditor(),
                new FileUrlEditor(),
                new FileMdEditor()
            };

            DirectoryEditors = new List<Editor<IDirectoryItem>>
            {
                new DefaultDirectoryEditor()
            };
        }

        public static Editor<IDirectoryItem> GetDefaultEditor(IDirectoryItem item)
        {
            return DirectoryEditors.LastOrDefault(e => e.CanHandle(item));
        }

        public static Editor<IDirectoryItem> GetDirectoryEditor(string name)
        {
            return DirectoryEditors.LastOrDefault(e => e.Name.Equals(name));
        }

        public static Editor<IFileItem> GetDefaultEditor(IFileItem item)
        {
            return FileEditors.LastOrDefault(e => e.CanHandle(item));
        }

        public static Editor<IFileItem> GetFileEditor(string name)
        {
            return FileEditors.LastOrDefault(e => e.Name.Equals(name));
        }
    }
}
