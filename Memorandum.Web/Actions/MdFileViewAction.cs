using System;

namespace Memorandum.Web.Actions
{
    class MdFileViewAction : FileBaseViewAction
    {
        public override string Editor => "markdown";
        public override string Template => "Files/md";

        public MdFileViewAction() : base(".md", ".markdown", ".text")
        {
        }
    }
}
