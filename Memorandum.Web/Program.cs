using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            var router = new Router();
            router.Bind("", Views.GeneralViews.Router);
            router.Bind("^/text", Views.TextNodeViews.Router);
            router.Bind("^/url", Views.UrlNodeViews.Router);
            router.Bind("^/file", Views.FileNodeViews.Router);
            router.Bind("^/api", ApiViews.Router);

            var app = new App(router);
            app.Listen(19000);
        }
    }
}
