using System.IO;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    internal class AnonymousRenderedLinkDrop : AnonymousLinkDrop
    {
        public string Rendered { get; private set; }

        public AnonymousRenderedLinkDrop(Node endnode) : base(endnode)
        {
            // TODO: replace "Templates" part with reference to some config field
            var tpl = Template.Parse(File.ReadAllText(Path.Combine("Templates", "Blocks", "_link.liquid")));
            Rendered = tpl.Render(Hash.FromAnonymousObject(new { link = this }));
        }

        public AnonymousRenderedLinkDrop(NodeDrop endnode) : base(endnode)
        {
            // TODO: replace "Templates" part with reference to some config field
            var tpl = Template.Parse(File.ReadAllText(Path.Combine("Templates", "Blocks", "_link.liquid")));
            Rendered = tpl.Render(Hash.FromAnonymousObject(new { link = this }));
        }
    }
}