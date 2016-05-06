using System.Collections.Generic;
using System.Linq;

namespace Memorandum.Web.Editors
{
    class CodeFileEditor : FileEditor
    {
        // TODO: REPLACE WITH EXTENSIONS
        private static readonly string[] EditableFileTypes =
        {
            "application/javascript",
            "application/json",
            "application/atom+xml",
            "application/rss+xml",
            "application/xml",
            "application/sparql-query",
            "application/sparql-results+xml",
            "text/calendar",
            "text/css",
            "text/csv",
            "text/html",
            "text/n3",
            "text/plain",
            "text/plain-bas",
            "text/prs.lines.tag",
            "text/richtext",
            "text/sgml",
            "text/tab-separated-values",
            "text/troff",
            "text/turtle",
            "text/uri-list",
            "text/vnd.curl",
            "text/vnd.curl.dcurl",
            "text/vnd.curl.mcurl",
            "text/vnd.curl.scurl",
            "text/vnd.fly",
            "text/vnd.fmi.flexstor",
            "text/vnd.graphviz",
            "text/vnd.in3d.3dml",
            "text/vnd.in3d.spot",
            "text/vnd.sun.j2me.app-descriptor",
            "text/vnd.wap.wml",
            "text/vnd.wap.wmlscript",
            "text/x-asm",
            "text/x-c",
            "text/x-fortran",
            "text/x-java-source,java",
            "text/x-pascal",
            "text/x-setext",
            "text/x-uuencode",
            "text/x-vcalendar",
            "text/x-vcard",
            "text/yaml"
        };

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

        public CodeFileEditor() : base(KnownExtensions)
        {
        }

        public override string Name => "code";
        public override string Template => "Files/code";
    }
}
