using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    static class Utilities
    {
        public static List<LinkDrop> GetLinks(UnitOfWork unit, Node node)
        {
            var links = unit.Links.Where(l => l.StartNode == node.NodeId.Id && l.StartNodeProvider == node.NodeId.Provider).ToList();
            var linkDrops = links.Select(link => new LinkDrop(link, unit.Nodes.FindById(link.GetEndIdentifier()))).ToList();
            
            var dir = node as DirectoryNode;
            if (dir != null)
            {
                linkDrops.AddRange(dir.GetChild().Select(ch => new LinkDrop(new Link(node, ch), ch)));
            }
            return linkDrops;
        }

        public static IEnumerable<LinksGroupDrop> GetGroupedLinks(UnitOfWork unit, Node node)
        {
            var linkDrops = GetLinks(unit, node);
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

        public static void DeleteLinks(UnitOfWork unit, Node node)
        {
            var outLinks = unit.Links.Where(
                l => l.StartNodeProvider == node.NodeId.Provider && l.StartNode == node.NodeId.Id);

            var inLinks = unit.Links.Where(
                l => l.EndNodeProvider == node.NodeId.Provider && l.EndNode == node.NodeId.Id);

            foreach (var inLink in inLinks)
                unit.Links.Delete(inLink);

            foreach (var outLink in outLinks)
                unit.Links.Delete(outLink);  
        }

        public static string GetWebPageTitle(string url)
        {
            // Create a request to the url
            var request = WebRequest.Create(url) as HttpWebRequest;

            // If the request wasn't an HTTP request (like a file), ignore it
            if (request == null) return null;

            // Use the user's credentials
            request.UseDefaultCredentials = true;

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response = null;
            try { response = request.GetResponse() as HttpWebResponse; }
            catch (WebException) { return null; }

            if (response == null)
                return null;

            response.Close();
            // Regular expression for an HTML title
            const string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            var headers = new List<string>(response.Headers.AllKeys);
            if (headers.Contains("Content-Type"))
            {
                if (response.Headers["Content-Type"].StartsWith("text/html"))
                {
                    // Download the page
                    var web = new WebClient
                    {
                        UseDefaultCredentials = true,
                        Encoding = System.Text.Encoding.UTF8
                    };

                    var page = web.DownloadString(url);

                    // Extract the title
                    var ex = new Regex(regex, RegexOptions.IgnoreCase);
                    return ex.Match(page).Value.Trim();
                }
            }

            // If content disposition fails
            if (headers.Contains("Content-Disposition"))
            {
                var cd = response.Headers["content-disposition"];
                if (!string.IsNullOrEmpty(cd))
                {
                    var filename = new ContentDisposition(cd).FileName;
                    if (!string.IsNullOrEmpty(filename))
                        return filename;
                }
            }

            return null;
        }
    }
}