using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Utitlities
{
    internal static class Utilities
    {
        public static List<LinkDrop> GetLinkDrops(UnitOfWork unit, Node node)
        {
            var links =
                unit.Links.Where(l => l.StartNode == node.NodeId.Id && l.StartNodeProvider == node.NodeId.Provider)
                    .ToList();
            var linkDrops =
                links.Select(link => new LinkDrop(link, unit.Nodes.FindById(link.GetEndIdentifier()))).ToList();

            var dir = node as DirectoryNode;
            if (dir != null)
            {
                linkDrops.AddRange(dir.GetChild().Select(ch => new LinkDrop(new Link(node, ch)
                {
                    Comment = ch.IsDirectory ? "Folder" : "File"
                }, ch)));
            }
            return linkDrops;
        }

        public static List<RenderedLinkDrop> GetRenderedLinkDrops(UnitOfWork unit, Node node)
        {
            var links =
                unit.Links.Where(l => l.StartNode == node.NodeId.Id && l.StartNodeProvider == node.NodeId.Provider)
                    .ToList();
            var linkDrops =
                links.Select(link => new RenderedLinkDrop(link, unit.Nodes.FindById(link.GetEndIdentifier()))).ToList();

            var dir = node as DirectoryNode;
            if (dir != null)
            {
                linkDrops.AddRange(dir.GetChild().Select(ch => new RenderedLinkDrop(new Link(node, ch)
                {
                    Comment = ch.IsDirectory ? "Folder" : "File"
                }, ch)));
            }
            return linkDrops;
        }

        public static IEnumerable<LinksGroupDrop> GetGroupedLinks(UnitOfWork unit, Node node)
        {
            var gId = 0;
            var linkDrops = GetLinkDrops(unit, node);
            if (linkDrops.Count == 0)
                return new List<LinksGroupDrop> {new LinksGroupDrop(gId) };

            var groups = new List<LinksGroupDrop>();
            var unnamedGroup = new LinksGroupDrop(gId++);
            groups.Add(unnamedGroup);

            foreach (var link in linkDrops)
            {
                if (!string.IsNullOrWhiteSpace(link.Comment))
                {
                    var sameRealtionLink =
                        unnamedGroup.Items.FirstOrDefault(
                            l =>
                                l.Comment != null &&
                                l.Comment.Equals(link.Comment, StringComparison.CurrentCultureIgnoreCase));
                    if (sameRealtionLink != null)
                    {
                        unnamedGroup.Items.Remove(sameRealtionLink);
                        var group = new LinksGroupDrop(gId++, new List<LinkDrop> {link, sameRealtionLink});
                        groups.Add(group);
                        continue;
                    }

                    var groupWithSameRelation =
                        groups.FirstOrDefault(
                            g => g.Name.Equals(link.Comment, StringComparison.CurrentCultureIgnoreCase));
                    if (groupWithSameRelation != null)
                    {
                        groupWithSameRelation.Items.Add(link);
                        continue;
                    }
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

            if (request == null) return null;

            // Use the user's credentials
            request.UseDefaultCredentials = true;

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException)
            {
                return Path.GetFileName(new Uri(url).LocalPath);
            }

            if (response == null)
                return Path.GetFileName(new Uri(url).LocalPath);


            // Regular expression for an HTML title
            const string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            var headers = new List<string>(response.Headers.AllKeys);
            
            // Try to get title from text html
            if (response.ContentType.StartsWith("text/html"))
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                var page = encoding.GetString(FromResponseStream(response.GetResponseStream()));
                var ex = new Regex(regex, RegexOptions.IgnoreCase);
                response.Close();
                return ex.Match(page).Value.Trim();
            }

            // Try to get title from pdf meta
            if (response.ContentType.Contains("pdf") || Path.GetExtension(response.ResponseUri.LocalPath).Equals("pdf"))
            {
                var pdf = new PdfReader(FromResponseStream(response.GetResponseStream()));
                if (pdf.Info.ContainsKey("Title"))
                {
                    var title = pdf.Info["Title"];
                    if (!string.IsNullOrWhiteSpace(title) && 
                        !title.Trim().ToLower().Equals("untitled"))
                        return title;
                }
            }
            response.Close();

            // Try to get title from content disposition
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

            // Get from URI finally
            return Path.GetFileName(response.ResponseUri.LocalPath);
        }

        public static Link CreateLinkForNode(IRequest request, Node parentNode, Node newNode)
        {
            var link = new Link(parentNode, newNode)
            {
                Comment = request.PostArgs["comment"],
                DateAdded = DateTime.Now,
                User = request.User
            };
            request.UnitOfWork.Links.Save(link);
            return link;
        }

        public static Link CreateLinkForNode(IRequest request, Node parentNode, Node newNode, string comment)
        {
            var link = new Link(parentNode, newNode)
            {
                Comment = comment,
                DateAdded = DateTime.Now,
                User = request.User
            };
            request.UnitOfWork.Links.Save(link);
            return link;
        }


        public static byte[] FromResponseStream(Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}