using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    internal class AnonymousLinkDrop : Drop, ILinkDrop
    {
        public AnonymousLinkDrop(Node endnode, string comment = "")
        {
            Node = NodeDropFactory.Create(endnode);
            Comment = comment;
        }

        public AnonymousLinkDrop(NodeDrop endnode, string comment = "")
        {
            Node = endnode;
            Comment = comment;
        }

        public int? Id => null;

        public string Comment { get; }

        public string StartNode => null;

        public string StartNodeProvider => null;

        public string EndNode => Node.NodeId;

        public string EndNodeProvider => Node.Provider;

        public NodeDrop Node { get; }
    }
}