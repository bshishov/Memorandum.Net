using FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    class RouteContextWithArg : IRouteContext
    {
        private readonly string[] _args;
        private readonly RouteWithArg _route;

        public RouteContextWithArg(RouteWithArg route, string[] args)
        {
            _args = args;
            _route = route;
        }
        public Response Proceed(Request request)
        {
            return _route.Proceed(request, _args);
        }
    }
}