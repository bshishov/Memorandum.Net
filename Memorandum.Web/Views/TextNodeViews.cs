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
    internal static class TextNodeViews
    {
        private static Response TextNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.Text.FindById(Convert.ToInt32(args[0]));
            if (node == null)
                throw new Http404Exception("Node not found");

            if (node.User.Id != user.Id)
                throw new Http404Exception("Access denied :)");

            return new TemplatedResponse("text_node", new
            {
                Title = "Home",
                Node = new TextNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }
    
        public static Router Router = new Router(new List<IRoute>
        {
            new RouteWithArg("/([0-9]+)$", TextNode),
        });
    }
}