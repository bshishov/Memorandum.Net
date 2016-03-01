using Memorandum.Core.Domain;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Views.Drops
{
    internal class RenderedLinkDrop : LinkDrop
    {
        public string Rendered { get; private set; }

        public RenderedLinkDrop(Link link, Node endnode) : base(link, endnode)
        {
            Rendered = TemplateUtilities.RenderTemplate("Blocks/link", new {link = this});
        }

        public RenderedLinkDrop(Link link, NodeDrop endnode) : base(link, endnode)
        {
            Rendered = TemplateUtilities.RenderTemplate("Blocks/link", new { link = this });
        }
    }
}