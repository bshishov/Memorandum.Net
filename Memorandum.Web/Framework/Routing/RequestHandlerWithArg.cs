using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal delegate Response RequestHandlerWithArg(IRequest request, params string[] args);
}