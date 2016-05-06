namespace Memorandum.Web.Editors
{
    class DefaultFileEditor : FileEditor
    {
        public DefaultFileEditor() : base()
        {
        }

        public override string Name => "default";
        public override string Template => "Files/default";
    }
}