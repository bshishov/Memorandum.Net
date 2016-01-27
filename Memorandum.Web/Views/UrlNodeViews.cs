using System.Collections.Generic;
using System.Linq;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    static class UrlNodeViews
    {
        public static Router Router = new Router(new List<IRoute>()
        {
            new RouteWithArg("/([a-z0-9]+)$", UrlNode),
        });

        static Response UrlNode(Request request, string[] args)
        {
            var node = Engine.Memo.URLNodes.Where(u => u.Hash == args[0]).FirstOrDefault();

            return new TemplatedResponse("url_node", new
            {
                Title = "Home",
                Node = new UrlNodeDrop(node),
                Links = Engine.GetGroupedLinks(node)
            });
        }

    }
}