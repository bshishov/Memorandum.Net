using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal delegate Response RequestHandlerWithArg(IRequest fastCGIRequest, params string[] args);
}