using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Creators;

namespace Memorandum.Web.Editors
{
    static class EditorManager
    {
        private static readonly List<IFileEditor> _editors;
        private static readonly List<ICreator> _creators;

        public static IFileEditor Default { get; }
        public static IEnumerable<IFileEditor> Editors => _editors;
        public static IEnumerable<ICreator> Creators => _creators;

        static EditorManager()
        {
            var defaultEditor = new DefaultFileEditor();

            _editors = new List<IFileEditor>
            {
                new UrlEditor(),
                new MarkdownFileEditor(),
                new CodeFileEditor(),
                defaultEditor,
            };

            _creators = new List<ICreator>
            {
                new UploaderCreator(),
                new URLCreator(),
                new DirectoryCreator(),
                new TextCreator(),
                new MarkdownCreator()
            };

            Default = defaultEditor;
        }

        public static IFileEditor GetEditor(string name)
        {
            return _editors.FirstOrDefault(e => e.Name.Equals(name));
        }

        public static IFileEditor GetEditor(IFileItem item)
        {
            var editor = _editors.FirstOrDefault(e => e.CanHandle(item));
            if (editor != null)
                return editor;

            return Default;
        }

        public static ICreator GetCreator(string id)
        {
            return _creators.FirstOrDefault(e => e.Id.Equals(id));
        }
    }
}
