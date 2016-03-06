using System;
using System.Diagnostics;
using FastCGI;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Backend.FastCGI
{
    class FastCGIBackend : IBackend
    {
        private readonly FCGIApplication _fcgiApplication;
        private IAsyncRequestHandler _handler;
        private readonly int _port;

        public FastCGIBackend(int port)
        {
            _port = port;
            _fcgiApplication = new FCGIApplication();
            _fcgiApplication.OnRequestReceived += FcgiApplicationOnOnRequestReceived;
        }

        private void FcgiApplicationOnOnRequestReceived(object sender, global::FastCGI.Request request)
        {
            var response = _handler.Execute(new FastCGIRequest(request)).Result;

            if (response == null) return;
            var httpResponse = response as HttpResponse;

            try
            {
                if(httpResponse != null)
                    request.WriteResponse(httpResponse.GetHeader());
                    
                request.WriteResponse(response.GetBody());
                request.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public void Run(IAsyncRequestHandler handler)
        {
            Debug.Assert(handler != null);
            _handler = handler;
            _fcgiApplication.Run(_port);
        }

        public void Stop()
        {
            _fcgiApplication.StopListening();
        }
    }
}
