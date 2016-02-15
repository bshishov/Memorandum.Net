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
    internal static class UrlNodeViews
    {
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
                var newNode = new URLNode
                {
                    DateAdded = DateTime.Now,
                    URL = request.PostArgs["url"],
                    User = user,
                    Name = name
                };

                request.UnitOfWork.URL.Save(newNode);
                Utilities.MakeRelationForNewNode(request, parentNode, newNode);
                return new RedirectResponse("/" + parentNode.NodeId.Provider + "/" + parentNode.NodeId.Id);
            }

            throw new Http404Exception("POST expected");
        }

        private static Response UrlNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.URL.FindById(Convert.ToInt32(args[0]));

            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            return new TemplatedResponse("url_node", new
            {
                Title = "Home",
                Node = new UrlNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("/add$", UrlNodeAdd),
            new RouteWithArg("/([0-9]+)$", UrlNode),
            new RouteWithArg("/([0-9]+)/delete$", UrlNodeDelete)
        });
    }
}