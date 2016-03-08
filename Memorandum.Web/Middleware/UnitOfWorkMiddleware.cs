using Memorandum.Core;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Middleware
{
    internal class UnitOfWorkMiddleware : IMiddleware
    {
        public void Handle(IRequest request)
        {
            request.UnitOfWork = new UnitOfWork();
        }

        public void Handle(IRequest request, Response response)
        {
            if (request.UnitOfWork != null)
            {
                request.UnitOfWork.Dispose();
                request.UnitOfWork = null;
            }
        }
    }
}