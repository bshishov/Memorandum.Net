namespace Memorandum.Web.Creators
{
    class MarkdownCreator : FileCreator
    {
        public override string Id => "markdown";
        public override string Name => "Markdown";
        public override string Template => "Blocks/Forms/text_file";
        public override string DefaultExtension => ".md";
    }
}