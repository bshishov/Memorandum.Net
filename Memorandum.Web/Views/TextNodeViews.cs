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
            new RouteWithArg("/([0-9]+)$", TextNode),
        });
      
        public static Response TextNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if(user == null)
                return new RedirectResponse("/login");

            var node = Engine.Memo.TextNodes.FindById(Convert.ToInt32(args[0]));
            if(node == null)
                throw new Http404Exception("Node not found");
         
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
    }
}
