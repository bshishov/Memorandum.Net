namespace Memorandum.Web.Actions
{
    class CodeFileViewAction : FileBaseViewAction
    {
        public static readonly string[] KnownExtensions =
        {
            ".txt",
            ".md",
            ".conf", ".log", ".ini", ".settings",
            ".c", ".cs", ".cpp", ".h",
            ".xml", ".json", "xaml", ".yaml", ".yml",
            ".py",
            ".css", ".js", ".html", ".less",
            ".java",
        };

        public override string Editor => "code";
        public override string Template => "Files/code";

        public CodeFileViewAction() : base(KnownExtensions)
        {
        }
    }
}
