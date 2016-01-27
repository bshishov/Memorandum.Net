using System;
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

        public static IEnumerable<LinksGroupDrop> GetGroupedLinks(Node node)
        {
            var links = Memo.Links.Where(l => l.StartNode == node.NodeId && l.StartNodeProvider == node.Provider).ToList();
            var linkDrops = new List<LinkDrop>();
            foreach (var link in links)
            {
                if (link.EndNodeProvider == "text")
                    linkDrops.Add(new LinkDrop(link, Memo.TextNodes.FindById(Convert.ToInt32(link.EndNode))));
                if (link.EndNodeProvider == "url")
                    linkDrops.Add(new LinkDrop(link, Memo.URLNodes.Where(u=>u.Hash == link.EndNode).First()));
                if (link.EndNodeProvider == "file")
                    linkDrops.Add(new LinkDrop(link, Memo.Files.FindById(link.EndNode)));
            }

            if (node.Provider == "file")
            {
                var dir = node as DirectoryNode;
                if (dir != null)
                {
                    linkDrops.AddRange(dir.GetChild().Select(ch => new LinkDrop(new Link(node,ch), ch)));
                }
            }

            if (linkDrops.Count == 0)
                return null;

            return new List<LinksGroupDrop>()
            {
                new LinksGroupDrop(linkDrops)
            };
        }
    }
}