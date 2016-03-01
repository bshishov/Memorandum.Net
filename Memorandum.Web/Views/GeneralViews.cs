using System;
using System.Collections.Generic;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;

namespace Memorandum.Web.Views
{
    internal static class GeneralViews
    {
        public static Response Home(Request request)
        {
            var userId = request.UserId;
            if (userId != null)
            {
                var user = request.User;
                if(user == null)
                    throw new InvalidOperationException("Invalid user in session");
                return new RedirectResponse("/text/" + user.Home.Id);
            }

            return new RedirectResponse("/login");
        }

        public static Response Login(Request request)
        {
            if (request.Method == "GET")
            {
                if (request.UserId != null)
                    return new RedirectResponse("/");

                return new TemplatedResponse("login", new
                {
                    Title = "Login"
                });
            }

            if (request.Method == "POST")
            {
                var user = request.UnitOfWork.Users.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user != null)
                {
                    request.UserId = user.Id;
                }
            }

            return new RedirectResponse("/");
        }

        public static Response Logout(Request request)
        {
            if (request.UserId != null)
            {
                request.UserId = null;
                return new RedirectResponse("/");
            }

            return new RedirectResponse("/");
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("^/$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout)
        });
    }
}