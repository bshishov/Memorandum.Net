using System.Collections.Generic;
using System.Linq;

namespace Memorandum.Web.Views.Drops
{
    internal class LinksGroupDrop : DropGroup<LinkDrop>
    {
        public int Id { get; private set; }

        public LinksGroupDrop(int id)
        {
            Id = id;
            Items = new List<LinkDrop>();
            Name = "";
        }

        public LinksGroupDrop(int id, IEnumerable<LinkDrop> linkDrops)
        {
            Id = id;
            Items = linkDrops.ToList();
            Name = Items.First().Comment;
        }

        public bool HasItems => Items != null && Items.Count > 0;
    }
}