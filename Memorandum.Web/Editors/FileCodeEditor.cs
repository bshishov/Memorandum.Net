using System;
using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    class FileCodeEditor : FileEditorBase
    {
        class CodeViewAction : FileBaseViewAction
        {
            public override string Template => "Files/code";
        }

        public static readonly string[] KnownExtensions =
        {
            ".txt",
            ".md",
            ".conf", ".log", ".ini", ".settings",
            ".c", ".cs", ".cpp", ".h",
            ".sh",
            ".xml", ".json", "xaml", ".yaml", ".yml", ".csv",
            ".py",
            ".css", ".js", ".html", ".less",
            ".java",
        };

        public override string Name => "code";

        public FileCodeEditor() : base(KnownExtensions, new CodeViewAction(), new FileSaveAction())
        {
        }
    }
}