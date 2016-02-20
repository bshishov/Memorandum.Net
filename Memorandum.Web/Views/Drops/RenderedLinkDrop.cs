using System.IO;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    internal class RenderedLinkDrop : LinkDrop
    {
        public string Rendered { get; private set; }

        public RenderedLinkDrop(Link link, Node endnode) : base(link, endnode)
        {
            // TODO: replace "Templates" part with reference to some config field
            var tpl = Template.Parse(File.ReadAllText(Path.Combine("Templates", "Blocks", "_link.liquid")));
            Rendered = tpl.Render(Hash.FromAnonymousObject(new { link = this }));
        }

        public RenderedLinkDrop(Link link, NodeDrop endnode) : base(link, endnode)
        {
            // TODO: replace "Templates" part with reference to some config field
            var tpl = Template.Parse(File.ReadAllText(Path.Combine("Templates", "Blocks", "_link.liquid")));
            Rendered = tpl.Render(Hash.FromAnonymousObject(new { link = this }));
        }
    }
}