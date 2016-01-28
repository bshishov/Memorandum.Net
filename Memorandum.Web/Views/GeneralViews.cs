using System.Collections.Generic;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Views
{
    static class GeneralViews
    {
        public static Router Router = new Router(new List<IRoute>()
        {
            new Route("^/$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout),
        });
        
        public static Response Home(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user != null)
            {
                return new RedirectResponse("/text/1");
            }
            
            return new RedirectResponse("/login");
        }

        public static Response Login(Request request)
        {
            if (request.Method == "GET")
            {
                var user = request.Session.Get<User>("user");
                if (user != null)
                    return new RedirectResponse("/");

                return new TemplatedResponse("login", new
                {
                    Title = "Login",
                });
            }

            if (request.Method == "POST")
            {
                var p = HttpUtilities.ParseQueryString(request.RawRequest.Body);
                var user = Engine.Memo.Auth(p["username"], p["password"]);
                if (user != null)
                {
                    request.Session.Set("user", user);
                }
            }

            return new RedirectResponse("/");
        }

        public static Response Logout(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user != null)
            {
                request.Session.Remove("user");
                return new RedirectResponse("/");
            }

            return new RedirectResponse("/");
        }
    }
}
