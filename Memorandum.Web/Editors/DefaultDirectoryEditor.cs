using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    class DefaultDirectoryEditor : Editor<IDirectoryItem>
    {
        public DefaultDirectoryEditor() : base(new DefaultDirectoryViewFactory(), 
            new ItemRenameAction(),
            new ItemDeleteAction(),
            new ItemShareAction(),
            new DirectoryCreateFileAction(),
            new DirectoryViewAction(),
            new DirectoryUploadAction()
            )
        {
        }

        public override string Name => "directory";

        public override bool CanHandle(IDirectoryItem item)
        {
            return item.Exists;
        }
    }
}