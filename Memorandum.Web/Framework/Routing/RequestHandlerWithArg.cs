using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Routing
{
    delegate Response RequestHandlerWithArg(Request request, params string[] args);
}