using System.Collections.Generic;
using System.Linq;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    static class Engine
    {
        public static Memo Memo = new Memo();

        public static List<LinkDrop> GetLinks(Node node)
        {
            var links = Memo.Links.Where(l => l.StartNode == node.NodeId.Id && l.StartNodeProvider == node.NodeId.Provider).ToList();
            var linkDrops = links.Select(link => new LinkDrop(link, Memo.Nodes.FindById(link.GetEndIdentifier()))).ToList();
            
            var dir = node as DirectoryNode;
            if (dir != null)
            {
                linkDrops.AddRange(dir.GetChild().Select(ch => new LinkDrop(new Link(node, ch), ch)));
            }
            return linkDrops;
        }

        public static IEnumerable<LinksGroupDrop> GetGroupedLinks(Node node)
        {
            var linkDrops = GetLinks(node);
            if (linkDrops.Count == 0)
                return new List<LinksGroupDrop>{ new LinksGroupDrop() };

            var groups = new List<LinksGroupDrop>();
            var unnamedGroup = new LinksGroupDrop();
            groups.Add(unnamedGroup);

            foreach (var link in linkDrops)
            {
                var sameRealtionLink = unnamedGroup.Items.FirstOrDefault(l => l.Relation == link.Relation);
                if (sameRealtionLink != null)
                {
                    unnamedGroup.Items.Remove(sameRealtionLink);
                    var group = new LinksGroupDrop(new List<LinkDrop> {link, sameRealtionLink});
                    groups.Add(group);
                    continue;
                }

                var groupWithSameRelation = groups.FirstOrDefault(g => g.Name == link.Relation);
                if (groupWithSameRelation != null)
                {
                    groupWithSameRelation.Items.Add(link);
                    continue;
                }
                
                unnamedGroup.Items.Add(link);
            }


            return groups;
        }
    }
}