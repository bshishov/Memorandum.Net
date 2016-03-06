using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Memorandum.Web.Framework.Responses;
using System.Threading;

namespace Memorandum.Web.Framework.Backend.HttpListener
{
    /// <summary>
    /// HttpRequest receiver backend
    /// Inspired by https://github.com/JamesDunne/aardwolf/blob/master/Aardwolf/HttpAsyncHost.cs
    /// </summary>
    class HttpListenerBackend : IBackend
    {
        private readonly System.Net.HttpListener _listener;
        readonly int _accepts;

        public HttpListenerBackend(int accepts, params string[] prefixes)
        {
            _accepts = accepts * Environment.ProcessorCount;
            _listener = new System.Net.HttpListener();
            foreach (var s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }
        }

        public HttpListenerBackend(params string[] prefixes) : this(4, prefixes)
        {
        }

        public void Run(IAsyncRequestHandler requestHandler)
        {
            Task.Run(() =>
            {
                try
                {
                    // Start the HTTP listener:
                    _listener.Start();
                }
                catch (HttpListenerException hlex)
                {
                    Console.Error.WriteLine(hlex.Message);
                    return;
                }

                // Accept connections:
                // Higher values mean more connections can be maintained yet at a much slower average response time; fewer connections will be rejected.
                // Lower values mean less connections can be maintained yet at a much faster average response time; more connections will be rejected.
                var sem = new Semaphore(_accepts, _accepts);

                while (true)
                {
                    sem.WaitOne();

#pragma warning disable 4014
                    _listener.GetContextAsync().ContinueWith(async (t) =>
                    {
                        string errMessage;

                        try
                        {
                            sem.Release();

                            var ctx = await t;
                            await ProcessListenerContext(ctx, requestHandler);
                            return;
                        }
                        catch (Exception ex)
                        {
                            errMessage = ex.ToString();
                        }

                        await Console.Error.WriteLineAsync(errMessage);
                    });
#pragma warning restore 4014
                }
            }).Wait();
        }
     
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        static async Task ProcessListenerContext(HttpListenerContext context, IAsyncRequestHandler handler)
        {
            Debug.Assert(context != null);
            Debug.Assert(handler != null);

            try
            {
                var response = await handler.Execute(new RequestWrapper(context.Request)) as HttpResponse;
                if (response != null)
                {
                    context.Response.StatusCode = response.StatusCode;
                    context.Response.Headers = response.Headers;
                    response.WriteBody(context.Response.OutputStream);
                }
                context.Response.Close();
            }
            catch (HttpListenerException)
            {
                // Ignored.
            }
            catch (Exception ex)
            {
                // TODO: better exception handling
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
