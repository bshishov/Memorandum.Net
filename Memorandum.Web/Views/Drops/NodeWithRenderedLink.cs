using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    internal class NodeWithLink
    {
        public NodeDrop Node { get; private set; }
        public ILinkDrop Link { get; private set; }

        public NodeWithLink(NodeDrop node, ILinkDrop renderedLink)
        {
            Node = node;
            Link = renderedLink;
        }

        public NodeWithLink(Node node, Link link) : this(NodeDropFactory.Create(node), new LinkDrop(link, node))
        {
        }

        public NodeWithLink(Node node) : this(NodeDropFactory.Create(node), new AnonymousLinkDrop(node))
        {
        }
    }


    internal class NodeWithRenderedLink : NodeWithLink
    {
        public NodeWithRenderedLink(NodeDrop node, RenderedLinkDrop renderedLink) : base(node, renderedLink)
        {
        }

        public NodeWithRenderedLink(Node node, Link link) : this(NodeDropFactory.Create(node), new RenderedLinkDrop(link, node))
        {
        }

        public NodeWithRenderedLink(Node node) : base(NodeDropFactory.Create(node), new AnonymousRenderedLinkDrop(node))
        {
        }
    }
}