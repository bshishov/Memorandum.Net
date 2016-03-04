using System;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Backend.HttpListener
{
    class HttpListenerBackend : IBackend
    {
        private RequestHandler _handler;
        private readonly System.Net.HttpListener _listener;

        public HttpListenerBackend(string[] prefixes)
        {
            _listener = new System.Net.HttpListener();
            foreach (var s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }
        }

        public void Listen(int port, RequestHandler handler)
        {
            _listener.Start();
            _handler = handler;
            Receive();
        }
        
        private void Receive()
        {
            var result = _listener.BeginGetContext(ListenerCallback, _listener);
            result.AsyncWaitHandle.WaitOne();
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            var listener = ar.AsyncState as System.Net.HttpListener;
            if (listener != null)
            {
                var context = listener.EndGetContext(ar);
                var response = _handler?.Invoke(new RequestWrapper(context.Request)) as HttpResponse;
                if (response != null)
                {
                    context.Response.StatusCode = response.StatusCode;
                    context.Response.Headers = response.Headers;
                    response.WriteBody(context.Response.OutputStream);
                }
                context.Response.Close();
            }
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
