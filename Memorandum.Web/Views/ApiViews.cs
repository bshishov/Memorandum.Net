using System.Collections.Generic;
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
            new RouteWithArg("/([a-z]+)/([^/.]+)$", NodeView),
            new RouteWithArg("/([a-z]+)/(.+)/links$", LinksView),
        });

        private static Response NodeView(Request request, string[] args)
        {
            var provider = args[0];
            var id = args[1];
            var node = Engine.Memo.Nodes.FindById(new NodeIdentifier(provider, id));
            return new HttpResponse(content: JsonConvert.SerializeObject(node, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }), status: 200, contenttype: "text/plain; charset=utf-8");
        }

        private static Response LinksView(Request request, params string[] args)
        {
            var provider = args[0];
            var id = args[1];
            var node = Engine.Memo.Nodes.FindById(new NodeIdentifier(provider, id));
            if (node == null)
                return new HttpResponse(content: "Resource not found", status: 404);

            //var links = Engine.Memo.Links.Where(l => l.StartNode == node.NodeId.Id && l.StartNodeProvider == node.NodeId.Provider).ToList();
            var links = Engine.GetGroupedLinks(node);
            return new HttpResponse(content: JsonConvert.SerializeObject(links, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }), status: 200, contenttype: "text/plain; charset=utf-8");
        }

        static Response ApiHome(Request request)
        {
            return new HttpResponse(content: "Not implemented", status:404);
        }
    }
}