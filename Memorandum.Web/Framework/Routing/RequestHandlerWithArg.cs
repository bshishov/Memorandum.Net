using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    internal delegate Response RequestHandlerWithArg(Request request, params string[] args);
}