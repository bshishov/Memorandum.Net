using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Memorandum.Core.Domain;
using Memorandum.Core.Search;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Middleware;
using Memorandum.Web.Utitlities;
using Memorandum.Web.Views.Drops;
using Memorandum.Web.Views.Providers;

namespace Memorandum.Web.Views.RestApi
{
    internal static class ApiViews
    {
        private static Response ApiHome(IRequest request)
        {
            // TODO: describe router | pass Router
            return new ApiResponse(new
            {
                Auth = "/auth (POST)",
                Search = "/search (GET)",
                NodesCollection = "/{provider} (GET, POST)",
                Node = "/{provider}/{id} (GET, PUT, DELETE)",
                NodeAction = "/{provider}/{id}/{action} (GET)"
            });
        }

        private static Response Search(IRequest request)
        {
            const string searchQueryKey = "q";

            if (request.UserId == null)
                return new NonAuthorizedApiResponse();

            var query = request.QuerySet[searchQueryKey];
            if (string.IsNullOrEmpty(query))
                return new BadRequestApiResponse("Empty query");
            var nodes = new List<Node>();
            nodes.AddRange(SearchManager.TextNodeIndex.Search(query));
            nodes.AddRange(SearchManager.UrlNodeIndex.Search(query));
            nodes.AddRange(SearchManager.FileNodeIndex.Search(query));
            if (string.IsNullOrEmpty(request.QuerySet["mode"]))
            {
                var drops = nodes.Select(NodeDropFactory.Create);
                return new ApiResponse(drops);
            }

            var results = nodes.Select(n => new RenderedNodeDrop(n)).ToList();
            return new ApiResponse(results);
        }

        private static Response Auth(IRequest request)
        {
            if (request.Method == "POST")
            {
                if (request.UserId != null)
                    return new BadRequestApiResponse("Already authorized");

                if (request.PostArgs["username"] == null || request.PostArgs["password"] == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var user = request.UnitOfWork.Users.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var token = Guid.NewGuid().ToString();
                ApiMiddleware.Tokens.Add(token, user.Id);
                return new ApiResponse(new { Token = token });
            }

            return new BadRequestApiResponse();
        }
        
        private static Response NodeLinksView(IRequest request, string[] args)
        {
            if (request.UserId == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User != null && node.User.Id != request.UserId)
                return new ForbiddenApiResponse();

            if (!string.IsNullOrEmpty(request.QuerySet["mode"]))
                return new ApiResponse(Utilities.GetRenderedLinkDrops(request.UnitOfWork, node));

            return new ApiResponse(Utilities.GetLinkDrops(request.UnitOfWork, node));
        }

        private static Response LinkView(IRequest request, string[] args)
        {
            if (request.UserId == null)
                return new NonAuthorizedApiResponse();

            if (string.IsNullOrEmpty(args[0]))
                return new BadRequestApiResponse();

            var link = request.UnitOfWork.Links.FindById(Convert.ToInt32(args[0]));

            if (link == null)
                return new ResourceNotFoundApiResponse();

            if (link.User.Id != request.UserId)
                return new ForbiddenApiResponse();

            if (request.Method == "DELETE")
            {
                request.UnitOfWork.Links.Delete(link);
                return new ApiResponse(statusMessage: "Link deleted");
            }

            return new BadRequestApiResponse();
        }
        
        private static Response LinksView(IRequest request)
        {
            if (request.UserId == null)
                return new NonAuthorizedApiResponse();
            
            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(
                    WebUtility.UrlDecode(request.PostArgs["parent_provider"]), 
                    WebUtility.UrlDecode(request.PostArgs["parent_id"]));
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);

                if (parentNode == null)
                    return new BadRequestApiResponse();

                if (parentNode.User.Id != request.UserId)
                    return new ForbiddenApiResponse();

                var results = new List<NodeWithRenderedLink>();
               
                var endNodeId = new NodeIdentifier(
                        WebUtility.UrlDecode(request.PostArgs["end_provider"]),
                        WebUtility.UrlDecode(request.PostArgs["end_id"]));
                var endNode = request.UnitOfWork.Nodes.FindById(endNodeId);

                if(endNode == null)
                    return new BadRequestApiResponse();

                results.Add(new NodeWithRenderedLink(endNode,
                    Utilities.CreateLinkForNode(request, parentNode, endNode)));

                return new ApiResponse(results, 201, "Nodes added");
            }

            return new BadRequestApiResponse();
        }

        private static Response Provider(IRequest request, params string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
                return new BadRequestApiResponse();

            if (request.User == null)
                return new NonAuthorizedApiResponse();

            var provider = ProviderManager.Get(args[0]);
            if (request.Method == "GET")
                return provider.ApiProviderViewGet(request);

            if (request.Method == "POST")
                return provider.ApiProviderViewPost(request);

            return new BadRequestApiResponse("Only GET and POST methods are supported");
        }

        private static Response ProviderNode(IRequest request, params string[] args)
        {
            if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
                return new BadRequestApiResponse();

            if (request.User == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User != null && node.User.Id != request.UserId)
                return new ForbiddenApiResponse("Access denied");

            var provider = ProviderManager.Get(args[0]);

            if (request.Method == "GET")
                return provider.ApiNodeViewGet(request, node);

            if (request.Method == "PUT")
                return provider.ApiNodeViewPut(request, node);

            if (request.Method == "DELETE")
                return provider.ApiNodeViewDelete(request, node);

            return new BadRequestApiResponse("Only GET, PUT and DELETE methods are supported");
        }

        private static Response ProviderNodeAction(IRequest request, params string[] args)
        {
            if (request.User == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User != null && node.User.Id != request.UserId)
                return new ForbiddenApiResponse("Access denied");

            var provider = ProviderManager.Get(args[0]);
            return provider.ApiNodeAction(request, node, args[2]);
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("/auth", Auth),
            new Route("/search", Search),

            new RouteWithArg("/links/([0-9]+)", LinkView),
            new Route("/links", LinksView),

            new RouteWithArg("/([a-z]+)/([^/]+)/([a-z]+)", ProviderNodeAction),
            new RouteWithArg("/([a-z]+)/([^/]+)", ProviderNode),
            new RouteWithArg("/([a-z]+)", Provider),

            new RouteWithArg("/([a-z]+)/(.+)/links", NodeLinksView),

            new Route("/", ApiHome),
        });
    }
}