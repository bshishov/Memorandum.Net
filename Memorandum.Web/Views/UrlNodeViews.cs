using System;
using System.Collections.Generic;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    internal static class UrlNodeViews
    {
        private static Response UrlNode(IRequest request, string[] args)
        {
            if (request.UserId == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.URL.FindById(Convert.ToInt32(args[0]));

            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != request.UserId)
                throw new Http404Exception("Access denied :)");

            return new TemplatedResponse("url_node", new
            {
                Title = node.Name,
                Node = new UrlNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new RouteWithArg("/([0-9]+)$", UrlNode),
        });
    }
}