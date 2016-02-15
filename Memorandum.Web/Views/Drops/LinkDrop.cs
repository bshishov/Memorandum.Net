using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    [DataContract]
    internal class LinkDrop : Drop
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

        [DataMember]
        public int Id => _link.Id;

        [DataMember]
        public string Comment => _link.Comment;

        [DataMember]
        public string StartNode => _link.StartNode;

        [DataMember]
        public string StartNodeProvider => _link.StartNodeProvider;

        [DataMember]
        public string EndNode => _link.EndNode;

        [DataMember]
        public string EndNodeProvider => _link.EndNodeProvider;

        [DataMember]
        public NodeDrop Node { get; }
    }

    internal class DropGroup<T> : Drop
    {
        public string Name { get; protected set; }
        public List<T> Items { get; protected set; }
    }

    internal class LinksGroupDrop : DropGroup<LinkDrop>
    {
        public LinksGroupDrop()
        {
            Items = new List<LinkDrop>();
            Name = "";
        }

        public LinksGroupDrop(IEnumerable<LinkDrop> linkDrops)
        {
            Items = linkDrops.ToList();
            Name = Items.First().Comment;
        }

        public bool HasItems => Items != null && Items.Count > 0;
    }
}