using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    class FileDefaultEditor : FileEditorBase
    {
        class BinaryFileViewAction : FileBaseViewAction
        {
            public override string Template => "Files/default";
        }

        public override string Name => "default"; // binary

        public FileDefaultEditor() : base(new BinaryFileViewAction())
        {
        }
    }
}