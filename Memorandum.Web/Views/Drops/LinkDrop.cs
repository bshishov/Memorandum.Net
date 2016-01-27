using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    class LinkDrop : Drop
    {
        private readonly Link _link;
        private readonly NodeDrop _endnode;

        public int Id { get { return _link.Id; } }
        public string Relation { get { return _link.Relation; } }
        public string StartNode { get { return _link.StartNode; } }
        public string StartNodeProvider { get { return _link.StartNodeProvider; } }
        public string EndNode { get { return _link.EndNode; } }
        public string EndNodeProvider { get { return _link.EndNodeProvider; } }

        public NodeDrop Node { get { return _endnode; } }

        public LinkDrop(Link link, Node endnode)
        {
            _link = link;
            if(EndNodeProvider == "text")
                _endnode = new TextNodeDrop(endnode as TextNode);
            if (EndNodeProvider == "url")
                _endnode = new UrlNodeDrop(endnode as URLNode);
            if (EndNodeProvider == "file")
            {
                if (((BaseFileNode) endnode).IsDirectory)
                {
                    _endnode = new DirectoryNodeDrop((BaseFileNode) endnode);
                    link.Relation = "Folder";
                }
                else
                {
                    _endnode = new FileNodeDrop((BaseFileNode) endnode);
                    link.Relation = "File";
                }
            }
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
        }

        public LinksGroupDrop(IEnumerable<LinkDrop> linkDrops)
        {
            Items = linkDrops.ToList();
            Name = Items.First().Relation;
        }
    }
}
