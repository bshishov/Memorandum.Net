namespace Memorandum.Web.Editors
{
    class MarkdownFileEditor : FileEditor
    {
        public MarkdownFileEditor() : base(".md", ".markdown", ".text")
        {
        }

        public override string Name => "markdown";
        public override string Template => "Files/md";
    }
}
