using Memorandum.Core.Domain;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Views.Drops
{
    internal class AnonymousRenderedLinkDrop : AnonymousLinkDrop
    {
        public string Rendered { get; private set; }

        public AnonymousRenderedLinkDrop(Node endnode) : base(endnode)
        {
            Rendered = TemplateUtilities.RenderTemplate("Blocks/link", new { link = this });
        }

        public AnonymousRenderedLinkDrop(NodeDrop endnode) : base(endnode)
        {
            Rendered = TemplateUtilities.RenderTemplate("Blocks/link", new { link = this });
        }
    }
}