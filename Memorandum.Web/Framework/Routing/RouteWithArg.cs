using System.Text.RegularExpressions;
using FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    class RouteWithArg : IRoute
    {
        public Regex Regex { get; private set; }

        private readonly RequestHandlerWithArg _handler;

        public RouteWithArg(string pattern, RequestHandlerWithArg handler)
        {
            Regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _handler = handler;
        }

        public Response Proceed(Request request, string[] args)
        {
            if (_handler != null)
                return _handler(request, args);

            return null;
        }
    }
}