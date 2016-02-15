using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotLiquid;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views.RestApi
{
    internal static class ApiViews
    {
        private static Response Search(Request request)
        {
            const string searchQueryKey = "q";

            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var query = request.QuerySet[searchQueryKey];
            if (string.IsNullOrEmpty(query))
                return new BadRequestApiResponse("Empty query");
            var nodes = request.UnitOfWork.Nodes.Search(query);
            if (string.IsNullOrEmpty(request.QuerySet["mode"]))
            {
                var drops = nodes.Select(NodeDropFactory.Create);
                return new ApiResponse(drops);
            }

            var tpl = Template.Parse(File.ReadAllText("Templates/Blocks/_link.liquid"));
            var results = (from node in nodes
                let l = new LinkDrop(new Link(node, node), node)
                select new SearchResult
                {
                    Node = NodeDropFactory.Create(node), Rendered = tpl.Render(Hash.FromAnonymousObject(new {link = l}))
                }).ToList();

            return new ApiResponse(results);
        }

        /// <summary>
        ///     TODO: Implement OAuth
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Response Auth(Request request)
        {
            if (request.Method == "POST")
            {
                var loggineduser = UserAuth(request, true);
                if (loggineduser == null)
                    return new BadRequestApiResponse("Already authorized");

                if (request.PostArgs["username"] == null || request.PostArgs["password"] == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var user = request.UnitOfWork.Users.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var token = Guid.NewGuid().ToString();
                Tokens.Add(token, user);
                return new ApiResponse(new {Token = token});
            }

            return new BadRequestApiResponse();
        }

        /// <summary>
        ///     Returns userobject by request token, if useCommonAuth eq 'true', then common auth (via sessions) would be preferred
        /// </summary>
        /// <param name="request"></param>
        /// <param name="useCommonAuth"></param>
        /// <returns></returns>
        private static User UserAuth(Request request, bool useCommonAuth)
        {
            if (useCommonAuth)
            {
                var user = request.Session.Get<User>("user");
                if (user != null)
                    return user;
            }

            const string tokenQueryStrigKey = "token";
            var token = request.QuerySet[tokenQueryStrigKey];
            if (string.IsNullOrEmpty(token))
                return null;
            if (Tokens.ContainsKey(token))
                return Tokens[token];
            return null;
        }

        private static Response NodeView(Request request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User.Id != user.Id)
                return new ForbiddenApiResponse();

            return new ApiResponse(NodeDropFactory.Create(node));
        }

        private static Response LinksView(Request request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User.Id != user.Id)
                return new ForbiddenApiResponse();

            return new ApiResponse(Utilities.GetLinks(request.UnitOfWork, node));
        }

        private static Response ApiHome(Request request)
        {
            return new ApiResponse(new
            {
                Node = "/:provider/:id",
                NodeLinks = "/:provider/:id/links"
            });
        }

        public class SearchResult
        {
            public NodeDrop Node { get; set; }
            public String Rendered { get; set; }
        }

        private static readonly Dictionary<string, User> Tokens = new Dictionary<string, User>();

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("/$", ApiHome),
            new Route("/auth$", Auth),
            new Route("/search$", Search),
            new RouteWithArg("/([a-z]+)/(.+)/links$", LinksView),
            new RouteWithArg("/([a-z]+)/([^/.]+)$", NodeView)
        });
    }
}