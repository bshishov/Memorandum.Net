using System;
using System.Collections.Generic;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Views.Providers;

namespace Memorandum.Web.Views
{
    internal static class GeneralViews
    {
        public static Response Home(IRequest request)
        {
            var userId = request.UserId;
            if (userId != null)
            {
                var user = request.User;
                if(user == null)
                    throw new InvalidOperationException("Invalid user in session");
                return new RedirectResponse("/text/" + user.Home.Id);
            }

            return new RedirectResponse("/login");
        }

        public static Response Login(IRequest request)
        {
            if (request.Method == "GET")
            {
                if (request.UserId != null)
                    return new RedirectResponse("/");

                return new TemplatedResponse("login", new
                {
                    Title = "Login"
                });
            }

            if (request.Method == "POST")
            {
                var user = request.UnitOfWork.Users.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user != null)
                {
                    request.UserId = user.Id;
                }
            }

            return new RedirectResponse("/");
        }

        public static Response Logout(IRequest request)
        {
            if (request.UserId != null)
            {
                request.UserId = null;
                return new RedirectResponse("/");
            }

            return new RedirectResponse("/");
        }

        private static Response ProviderNode(IRequest request, string[] args)
        {
            if (request.UserId == null)
                return new RedirectResponse("/login");

            if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
                throw new InvalidOperationException("Missing args");

            if (request.User == null)
                throw new InvalidOperationException("Unauthorized");

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User != null && node.User.Id != request.UserId)
                throw new InvalidOperationException("Access denied :)");

            var provider = ProviderManager.Get(args[0]);
            return provider.NodeView(request, node);
        }

        private static Response ProviderNodeAction(IRequest request, string[] args)
        {
            if (request.UserId == null)
                return new RedirectResponse("/login");

            if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
                throw new InvalidOperationException("Missing args");

            if (request.User == null)
                throw new InvalidOperationException("Unauthorized");

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User != null && node.User.Id != request.UserId)
                throw new InvalidOperationException("Access denied :)");

            var provider = ProviderManager.Get(args[0]);
            return provider.NodeAction(request, node, args[2]);
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("^/$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout),
            new RouteWithArg("/([a-z]+)/([^/]+)/([a-z]+)", ProviderNodeAction),
            new RouteWithArg("/([a-z]+)/([^/]+)", ProviderNode),
        });
    }
}