using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Backend
{
    delegate Response RequestHandler(IRequest request);

    interface IBackend
    {
        void Listen(int port, RequestHandler handler);
        void Stop();
    }
}
