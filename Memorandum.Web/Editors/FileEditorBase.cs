using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    abstract class FileEditorBase : Editor<IFileItem>
    {
        public static readonly IItemAction<IFileItem>[] DefaultActions = {
            new ItemRenameAction(),
            new ItemDeleteAction(),
            new ItemShareAction(),
            new FileDownloadAction(),
            new FileRawAction()
        };

        private readonly string[] _extensions;

        protected FileEditorBase(IViewFactory<IFileItem> viewFactory, string[] extensions, params IItemAction<IFileItem>[] actions) :
            base(viewFactory, DefaultActions.ToList().Union(actions).ToArray())
        {
            _extensions = extensions;
        }

        protected FileEditorBase(string[] extensions, params IItemAction<IFileItem>[] actions) 
            : this(new DefaultFileViewFactory(), extensions, actions)
        {
        }

        protected FileEditorBase(params string[] extensions)
            : this(new DefaultFileViewFactory(), extensions)
        {
        }

        protected FileEditorBase(params IItemAction<IFileItem>[] actions)
            : this(new DefaultFileViewFactory(), new string[] {}, actions)
        {
        }

        public override bool CanHandle(IFileItem item)
        {
            return item.Exists && (_extensions.Length == 0 || _extensions.Contains(item.Extension));
        }
    }
}