using System;
using System.Collections.Generic;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    static class TextNodeViews
    {
        public static Router Router = new Router(new List<IRoute>()
        {
            new Route("/add$", TextNodeAdd),
            new RouteWithArg("/([0-9]+)$", TextNode),
            new RouteWithArg("/([0-9]+)/delete$", TextNodeDelete),
        });

        private static Response TextNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if(user == null)
                return new RedirectResponse("/login");

            var node = Engine.Memo.TextNodes.FindById(Convert.ToInt32(args[0]));
            if(node == null)
                throw new Http404Exception("Node not found");

            if(node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");
         
            if (request.Method == "POST")
            {
                var p = HttpUtilities.ParseQueryString(request.RawRequest.Body);
                node.Text = p["text"];
                Engine.Memo.TextNodes.Save(node);
            }

            return new TemplatedResponse("text_node", new {
                Title = "Home",
                Node = new TextNodeDrop(node),
                Links = Engine.GetGroupedLinks(node)
            });
        }

        private static Response TextNodeDelete(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = Engine.Memo.TextNodes.FindById(Convert.ToInt32(args[0]));
            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            if (node.Id == user.Home.Id)
                throw new Http404Exception("Cannot delete own home");

            Engine.Memo.TextNodes.Delete(node);
            return new RedirectResponse("/");
        }

        private static Response TextNodeAdd(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            if (request.Method == "POST")
            {
                var p = HttpUtilities.ParseQueryString(request.RawRequest.Body);
                var parentNodeId = new NodeIdentifier(p["parent_provider"], p["parent_id"]);
                var parentNode = Engine.Memo.Nodes.FindById(parentNodeId);
                if (parentNode == null || p["text"] == null || p["relation"] == null)
                    throw new Http500Exception("Incorect parameters");

                var newNode = new TextNode()
                {
                    DateAdded = DateTime.Now, 
                    Text = p["text"], 
                    User = user
                };
                Engine.Memo.TextNodes.Save(newNode);
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
    }
}
