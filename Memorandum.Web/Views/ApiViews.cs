using System;
using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Newtonsoft.Json;

namespace Memorandum.Web.Views
{
    static class ApiViews
    {
        public static Router Router = new Router(new List<IRoute>()
        {
            new Route("/$", ApiHome),
            new RouteWithArg("/links/([a-z]+)/(.+)$", LinksView),
        });

        private static Response LinksView(Request request, params string[] args)
        {
            var provider = args[0];
            var id = args[1];
            Node node = null;

            if (provider == "text")
                node = Engine.Memo.TextNodes.FindById(Convert.ToInt32(id));

            if (provider == "url")
                node = Engine.Memo.URLNodes.Where(u => u.Hash == id).First();

            if (provider == "file")
                node = Engine.Memo.Files.FindById(id);

            if (node == null)
                return new HttpResponse(content: "Resource not found", status: 404);

            //var links = Engine.Memo.Links.Where(l => l.StartNode == node.NodeId && l.StartNodeProvider == node.Provider).ToList();
            var links = Engine.GetGroupedLinks(node);
            return new HttpResponse(content: JsonConvert.SerializeObject(links, Formatting.Indented), status: 200, contenttype: "text/plain; charset=utf-8");
        }

        static Response ApiHome(Request request)
        {
            return new HttpResponse(content: "Not implemented", status:404);
        }
    }
}