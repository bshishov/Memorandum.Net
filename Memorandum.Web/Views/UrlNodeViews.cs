using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    static class UrlNodeViews
    {
        public static Router Router = new Router(new List<IRoute>()
        {
            new Route("/add$", UrlNodeAdd),
            new RouteWithArg("/([0-9]+)$", UrlNode),
             new RouteWithArg("/([0-9]+)/delete$", UrlNodeDelete),

        });

        private static Response UrlNodeDelete(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = Engine.Memo.URLNodes.FindById(Convert.ToInt32(args[0]));
            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            Engine.Memo.URLNodes.Delete(node);
            return new RedirectResponse("/");
        }

        private static Response UrlNodeAdd(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            if (request.Method == "POST")
            {
                var p = HttpUtilities.ParseQueryString(request.RawRequest.Body);
                var parentNodeId = new NodeIdentifier(p["parent_provider"], p["parent_id"]);
                var parentNode = Engine.Memo.Nodes.FindById(parentNodeId);
                if (parentNode == null || p["url"] == null)
                    throw new Http500Exception("Incorect parameters");

                var name = GetWebPageTitle(p["url"]);
                var newNode = new URLNode()
                {
                    DateAdded = DateTime.Now,
                    URL = p["url"],
                    User = user,
                    Name = name
                };

                Engine.Memo.URLNodes.Save(newNode);
                var link = new Link(parentNode, newNode)
                {
                    Relation = p["relation"].ToLower(),
                    DateAdded = DateTime.Now,
                    User = user
                };
                Engine.Memo.Links.Save(link);

                if (p["relation_back"] != null)
                {
                    var linkBack = new Link(newNode, parentNode)
                    {
                        Relation = p["relation_back"].ToLower(),
                        DateAdded = DateTime.Now,
                        User = user
                    };
                    Engine.Memo.Links.Save(linkBack);
                }

                return new RedirectResponse("/" + parentNode.NodeId.Provider + "/" + parentNode.NodeId.Id);
            }

            throw new Http404Exception("POST expected");
        }

        static Response UrlNode(Request request, string[] args)
        {
            var node = Engine.Memo.URLNodes.FindById(Convert.ToInt32(args[0]));
            return new TemplatedResponse("url_node", new
            {
                Title = "Home",
                Node = new UrlNodeDrop(node),
                Links = Engine.GetGroupedLinks(node)
            });
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
            
            if(response == null)
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
            if(headers.Contains("Content-Disposition"))
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