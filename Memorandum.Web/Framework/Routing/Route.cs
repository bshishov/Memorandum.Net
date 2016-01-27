using System.Text.RegularExpressions;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    class Route : IRoute
    {
        public Regex Regex { get; private set; }

        private readonly RequestHandler _handler;

        public Route(string pattern, RequestHandler handler)
        {
            Regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _handler = handler;
        }

        public Response Proceed(Request request)
        {
            if (_handler != null)
                return _handler(request);

            return null;
        }
    }
}