namespace Memorandum.Web.Creators
{
    class TextCreator : FileCreator
    {
        public override string Id => "text";
        public override string Name => "Text File";
        public override string Template => "Blocks/Forms/text_file";
        public override string DefaultExtension => ".txt";
    }
}