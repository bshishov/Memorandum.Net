using System;
using System.Reflection;
using DotLiquid;
using DotLiquid.FileSystems;
using DotLiquid.NamingConventions;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;
using System.Diagnostics;
using Memorandum.Web.Framework.Backend;
using Memorandum.Web.Framework.Backend.FastCGI;
using Memorandum.Web.Framework.Backend.HttpListener;

namespace Memorandum.Web.Framework
{
    internal class App
    {
        private readonly Pipeline<IRequest, Response> _afterView = new Pipeline<IRequest, Response>();
        private readonly Pipeline<IRequest> _beforeView = new Pipeline<IRequest>();
        private readonly IBackend _backend;
        private readonly Router _rootRouter;

        public App(Router router)
        {
            _rootRouter = router;

            var assembly = Assembly.GetExecutingAssembly();
            Template.RegisterTag<Tags.StaticTag>("static");
            Template.FileSystem = new EmbeddedFileSystem(assembly, "Memorandum.Web.Templates");
            Template.NamingConvention = new CSharpNamingConvention();
            Template.RegisterFilter(typeof (Filters));

            //_backend = new FastCGIBackend();
            _backend = new HttpListenerBackend(new[] {"http://127.0.0.1:8000/"});
        }

        public void RegisterMiddleware(IMiddleware middleware)
        {
            _beforeView.Add(middleware);
            _afterView.Insert(0, middleware);
        }

        private Response RequestHandler(IRequest request)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                _beforeView.Run(request);

                var routeContext = _rootRouter.GetRoute(request.Path);

                if (routeContext != null)
                {
                    var response = routeContext.Proceed(request);
                    _afterView.Run(request, response);

                    var httpResponse = response as HttpResponse;
                    if (httpResponse != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

#if DEBUG
                        stopWatch.Stop();
                        Console.WriteLine("{0} {1} {2} {3}ms", request.Method, httpResponse.StatusCode, request.Path, stopWatch.ElapsedMilliseconds);
#else
                        Console.WriteLine("{0} {1} {2}", request.Method, httpResponse.StatusCode, request.Path);
#endif
                        Console.ResetColor();
                    }

                    return response;
                }

                throw new Http404Exception("No route found");
            }
            catch (HttpErrorException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1} {2} {3}", request.Method, e.StatusCode, request.Path, e.Message);
                Console.ResetColor();
                return new TemplatedResponse("error", new
                {
                    Title = "Server error",
                    ErrorCode = e.StatusCode,
                    ErrorMessage = e.ToString()
                }, e.StatusCode);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1} {2} {3}", request.Method, 500, request.Path, e.Message);
                Console.ResetColor();

                return new TemplatedResponse("error", new
                {
                    Title = "Server error",
                    ErrorCode = 500,
                    ErrorMessage = e.ToString()
                }, 500);
            }
        }

        public void Listen(int port)
        {
            _backend.Listen(port, RequestHandler);
        }
    }
}