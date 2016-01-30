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
            
            var node = request.UnitOfWork.Text.FindById(Convert.ToInt32(args[0]));
            if(node == null)
                throw new Http404Exception("Node not found");

            if(node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");
         
            if (request.Method == "POST")
            {
                node.Text = request.PostArgs["text"];
                request.UnitOfWork.Text.Save(node);
            }

            return new TemplatedResponse("text_node", new {
                Title = "Home",
                Node = new TextNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        private static Response TextNodeDelete(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.Text.FindById(Convert.ToInt32(args[0]));
            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            if (node.Id == user.Home.Id)
                throw new Http404Exception("Cannot delete own home");

            Utilities.DeleteLinks(request.UnitOfWork, node);
            request.UnitOfWork.Text.Delete(node);
            return new RedirectResponse("/");
        }

        private static Response TextNodeAdd(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(request.PostArgs["parent_provider"], request.PostArgs["parent_id"]);
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);
                if (parentNode == null || request.PostArgs["text"] == null || request.PostArgs["relation"] == null)
                    throw new Http500Exception("Incorect parameters");

                var newNode = new TextNode()
                {
                    DateAdded = DateTime.Now,
                    Text = request.PostArgs["text"], 
                    User = user
                };
                request.UnitOfWork.Text.Save(newNode);
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
    }
}
