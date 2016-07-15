using System;
using System.Collections.Generic;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Middleware;
using Shine;
using Shine.Responses;
using Shine.Routing;

namespace Memorandum.Web.Views.RestApi
{
    internal static class ApiViews
    {
        private static IResponse ApiHome(IRequest request)
        {
            // TODO: describe OldRouter | pass OldRouter
            return new ApiResponse(new
            {
                Auth = "/auth (POST)",
                Search = "/search (GET)",
                NodesCollection = "/{provider} (GET, POST)",
                Node = "/{provider}/{id} (GET, PUT, DELETE)",
                NodeAction = "/{provider}/{id}/{action} (GET)"
            });
        }

     
        private static IResponse Auth(IRequest request)
        {
            if (request.Method == "POST")
            {
                if (((CustomSessionContext)request.Session).User != null)
                    return new BadRequestApiResponse("Already authorized");

                if (request.PostArgs["username"] == null || request.PostArgs["password"] == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var user = UserManager.AuthByPassword(request.PostArgs["username"], request.PostArgs["password"]);
                if (user == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var token = Guid.NewGuid().ToString();
                ApiMiddleware.Tokens.Add(token, user.Name);
                return new ApiResponse(new { Token = token });
            }

            return new BadRequestApiResponse();
        }

        public static Router Router = new Router("^/api",
            new Route("/auth", Auth),
            new Route("/", ApiHome)
        );
    }
}