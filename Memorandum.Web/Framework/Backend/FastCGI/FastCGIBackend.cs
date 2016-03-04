using System;
using FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Backend.FastCGI
{
    class FastCGIBackend : IBackend
    {
        private readonly FCGIApplication _fcgiApplication;
        private RequestHandler _handler;

        public FastCGIBackend()
        {
            _fcgiApplication = new FCGIApplication();
            _fcgiApplication.OnRequestReceived += FcgiApplicationOnOnRequestReceived;
        }

        private void FcgiApplicationOnOnRequestReceived(object sender, global::FastCGI.Request request)
        {
            var response = _handler?.Invoke(new FastCGIRequest(request));
            if (response != null)
            {
                var httpResponse = response as HttpResponse;
                try
                {
                    if(httpResponse != null)
                        request.WriteResponse(httpResponse.GetHeader());
                    
                    request.WriteResponse(response.GetBody());
                    request.Close();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void Listen(int port, RequestHandler handler)
        {
            _handler = handler;
            _fcgiApplication.Run(port);
        }

        public void Stop()
        {
            _fcgiApplication.StopListening();
        }
    }
}
