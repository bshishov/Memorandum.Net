using System;
using System.Collections.Generic;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
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

            var node = request.UnitOfWork.URL.FindById(Convert.ToInt32(args[0]));
            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            Utilities.DeleteLinks(request.UnitOfWork, node);
            request.UnitOfWork.URL.Delete(node);
            return new RedirectResponse("/");
        }

        private static Response UrlNodeAdd(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(request.PostArgs["parent_provider"], request.PostArgs["parent_id"]);
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);
                if (parentNode == null || request.PostArgs["url"] == null)
                    throw new Http500Exception("Incorect parameters");

                var name = Utilities.GetWebPageTitle(request.PostArgs["url"]);
                var newNode = new URLNode()
                {
                    DateAdded = DateTime.Now,
                    URL = request.PostArgs["url"],
                    User = user,
                    Name = name
                };

                request.UnitOfWork.URL.Save(newNode);
                var link = new Link(parentNode, newNode)
                {
                    Relation = request.PostArgs["relation"].ToLower(),
                    DateAdded = DateTime.Now,
                    User = user
                };
                request.UnitOfWork.Links.Save(link);

                if (request.PostArgs["relation_back"] != null)
                {
                    var linkBack = new Link(newNode, parentNode)
                    {
                        Relation = request.PostArgs["relation_back"].ToLower(),
                        DateAdded = DateTime.Now,
                        User = user
                    };
                    request.UnitOfWork.Links.Save(linkBack);
                }

                return new RedirectResponse("/" + parentNode.NodeId.Provider + "/" + parentNode.NodeId.Id);
            }

            throw new Http404Exception("POST expected");
        }

        static Response UrlNode(Request request, string[] args)
        {
            var node = request.UnitOfWork.URL.FindById(Convert.ToInt32(args[0]));
            return new TemplatedResponse("url_node", new
            {
                Title = "Home",
                Node = new UrlNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }
    }
}