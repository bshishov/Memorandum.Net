using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    class FileMdEditor : FileEditorBase
    {
        public static readonly string[] KnownExtensions =
        {
            ".md", ".markdown", ".text"
        };

        class MdFileViewAction : FileBaseViewAction
        {
            public override string Template => "Files/md";
        }

        public override string Name => "markdown";

        public FileMdEditor() : base(KnownExtensions, new MdFileViewAction(), new FileSaveAction())
        {
        }
    }
}