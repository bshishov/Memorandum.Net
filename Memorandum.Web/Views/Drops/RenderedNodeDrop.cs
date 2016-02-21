using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public class RenderedNodeDrop : NodeDrop
    {
        public String Rendered { get; private set; }

        public RenderedNodeDrop(Node node) : base(node)
        {
            var link = new AnonymousRenderedLinkDrop(node);
            Rendered = link.Rendered;
        }
    }
}