using FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal interface IRouteContext
    {
        Response Proceed(Request request);
    }
}