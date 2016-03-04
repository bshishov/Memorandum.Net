using System.Text.RegularExpressions;
using Memorandum.Web.Framework.Backend.FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal class RouteWithArg : IRoute
    {
        private readonly RequestHandlerWithArg _handler;

        public RouteWithArg(string pattern, RequestHandlerWithArg handler)
        {
            Regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _handler = handler;
        }

        public Regex Regex { get; }

        public Response Proceed(IRequest request, string[] args)
        {
            if (_handler != null)
                return _handler(request, args);

            return null;
        }
    }
}