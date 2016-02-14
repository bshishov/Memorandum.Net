﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    [DataContract]
    class LinkDrop : Drop
    {
        private readonly Link _link;
        private readonly NodeDrop _endnode;

        [DataMember]
        public int Id { get { return _link.Id; } }

        [DataMember]
        public string Comment { get { return _link.Comment; } }

        [DataMember]
        public string StartNode { get { return _link.StartNode; } }

        [DataMember]
        public string StartNodeProvider { get { return _link.StartNodeProvider; } }

        [DataMember]
        public string EndNode { get { return _link.EndNode; } }

        [DataMember]
        public string EndNodeProvider { get { return _link.EndNodeProvider; } }

        [DataMember]
        public NodeDrop Node { get { return _endnode; } }

        public LinkDrop(Link link, Node endnode)
        {
            _link = link;
            _endnode = NodeDropFactory.Create(endnode);
        }

        public LinkDrop(Link link, NodeDrop endnode)
        {
            _link = link;
            _endnode = endnode;
        }
    }

    class DropGroup<T> : Drop
    {
        public string Name { get; protected set; }
        public List<T> Items { get; protected set; }
    }

    class LinksGroupDrop : DropGroup<LinkDrop>
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

        public bool HasItems
        {
            get { return Items != null && Items.Count > 0; }
        }
    }
}
