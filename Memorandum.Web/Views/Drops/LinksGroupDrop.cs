using System.Collections.Generic;
using System.Linq;

namespace Memorandum.Web.Views.Drops
{
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