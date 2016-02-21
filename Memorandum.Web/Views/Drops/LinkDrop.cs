using System;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    internal class LinkDrop : Drop, ILinkDrop
    {
        private readonly Link _link;

        public LinkDrop(Link link, Node endnode)
        {
            _link = link;
            Node = NodeDropFactory.Create(endnode);
        }

        public LinkDrop(Link link, NodeDrop endnode)
        {
            _link = link;
            Node = endnode;
        }

        public int? Id => _link.Id;

        public string Comment => _link.Comment;

        public string StartNode => _link.StartNode;

        public string StartNodeProvider => _link.StartNodeProvider;

        public string EndNode => _link.EndNode;

        public string EndNodeProvider => _link.EndNodeProvider;

        public DateTime DateAdded => _link.DateAdded;

        public NodeDrop Node { get; }
    }
}