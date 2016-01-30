using System.Linq;
using Memorandum.Core;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Middleware;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("createschema"))
                Database.CreateSchema();
            else
            {
                var router = new Router();
                router.Bind("", Views.GeneralViews.Router);
                router.Bind("^/text", Views.TextNodeViews.Router);
                router.Bind("^/url", Views.UrlNodeViews.Router);
                router.Bind("^/file", Views.FileNodeViews.Router);
                router.Bind("^/api", ApiViews.Router);

                var app = new App(router);
                app.RegisterMiddleware(new SessionMiddleware());
                app.RegisterMiddleware(new UnitOfWorkMiddleware());
                app.Listen(19000);
            }
        }
    }
}
