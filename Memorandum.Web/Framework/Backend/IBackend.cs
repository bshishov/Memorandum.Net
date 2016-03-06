using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Backend
{
    interface IBackend
    {
        void Run(IAsyncRequestHandler requestHandler);
        void Stop();
    }
}
