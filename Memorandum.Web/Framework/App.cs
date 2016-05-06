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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Memorandum.Web.Framework.Backend;

namespace Memorandum.Web.Framework
{
    internal class App : IAsyncRequestHandler
    {
        private readonly IBackend _backend;
        private readonly Pipeline<IRequest, Response> _afterView = new Pipeline<IRequest, Response>();
        private readonly Pipeline<IRequest> _beforeView = new Pipeline<IRequest>();
        private readonly Router _rootRouter;

        public App(IBackend backend, Router router)
        {
            _backend = backend;
            _rootRouter = router;

            
            Template.RegisterTag<Tags.StaticTag>("static");
#if DEBUG
            Template.FileSystem = new DebugFileSystem(Path.Combine("..", "..", "Templates"));
#else
            var assembly = Assembly.GetExecutingAssembly();
            Template.FileSystem = new EmbeddedFileSystem(assembly, "Memorandum.Web.Templates");
#endif
            Template.NamingConvention = new CSharpNamingConvention();
            Template.RegisterFilter(typeof (Filters));

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public void RegisterMiddleware(IMiddleware middleware)
        {
            _beforeView.Add(middleware);
            _afterView.Insert(0, middleware);
        }

        public async Task<Response> Execute(IRequest request)
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
#if DEBUG
                        stopWatch.Stop();
                        Console.WriteLine("{0} {1} {2} {3}ms", request.Method, httpResponse.StatusCode, request.Path, stopWatch.ElapsedMilliseconds);
#else
                        Console.WriteLine("{0} {1} {2}", request.Method, httpResponse.StatusCode, request.Path);
#endif
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
                    ErrorMessage = e.Message,
                    Exception = e.ToString()
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
                    ErrorMessage = e.Message,
                    Exception = e.ToString()
                }, 500);
            }
        }

        public void Run()
        {
            _backend.Run(this);
        }
    }
}