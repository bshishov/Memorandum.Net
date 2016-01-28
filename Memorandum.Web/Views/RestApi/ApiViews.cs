using System;
using System.Collections.Generic;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views.RestApi
{
    static class ApiViews
    {
        private static readonly Dictionary<string, User> Tokens = new Dictionary<string, User>();

        public static Router Router = new Router(new List<IRoute>()
        {
            new Route("/$", ApiHome),
            new Route("/auth$", Auth),
            new RouteWithArg("/([a-z]+)/(.+)/links$", LinksView),
            new RouteWithArg("/([a-z]+)/([^/.]+)$", NodeView),
        });

        /// <summary>
        /// TODO: Implement OAuth
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
                var p = HttpUtilities.ParseQueryString(request.RawRequest.Body);
                if(p["username"] == null || p["password"] == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var user = Engine.Memo.Auth(p["username"], p["password"]);
                if(user == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var token = Guid.NewGuid().ToString();
                Tokens.Add(token, user);
                return new ApiResponse(new { Token = token });
            }

            return new BadRequestApiResponse();
        }

        /// <summary>
        /// Returns userobject by request token, if useCommonAuth eq 'true', then common auth (via sessions) would be preferred
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

            var qs = HttpUtilities.ParseQueryString(request.QueryString);
            const string tokenQueryStrigKey = "token";
            var token = qs[tokenQueryStrigKey];
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

            var node = Engine.Memo.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

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
            
            var node = Engine.Memo.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User.Id != user.Id)
                return new ForbiddenApiResponse();
            
            return new ApiResponse(Engine.GetGroupedLinks(node));
        }

        static Response ApiHome(Request request)
        {
            return new ApiResponse(new
            {
                Node = "/:provider/:id",
                NodeLinks = "/:provider/:id/links"
            });
        }
    }
}