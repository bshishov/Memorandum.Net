using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal class RouteContext : IRouteContext
    {
        private readonly Route _route;

        public RouteContext(Route route)
        {
            _route = route;
        }

        public Response Proceed(IRequest request)
        {
            return _route.Proceed(request);
        }
    }
}