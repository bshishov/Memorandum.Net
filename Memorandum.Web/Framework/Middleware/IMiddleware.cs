using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework.Middleware
{
    internal interface IMiddleware : IHandler<IRequest>, IHandler<IRequest, Response>
    {
    }
}