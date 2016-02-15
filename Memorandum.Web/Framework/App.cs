using System;
using System.IO;
using DotLiquid;
using DotLiquid.FileSystems;
using DotLiquid.NamingConventions;
using FastCGI;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework
{
    internal class App
    {
        private readonly Pipeline<Request, Response> _afterView = new Pipeline<Request, Response>();
        private readonly Pipeline<Request> _beforeView = new Pipeline<Request>();
        private readonly FCGIApplication _fcgiApplication;
        private readonly Router _rootRouter;

        public App(Router router)
        {
            _rootRouter = router;

            // Template engine initialization
#if DEBUG
            var templatesDir =
                Path.Combine(
                    Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName,
                    "Templates");
#else
            var templatesDir = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
#endif
            Template.RegisterTag<Tags.StaticTag>("static");
            Template.FileSystem = new LocalFileSystem(templatesDir);
            Template.NamingConvention = new CSharpNamingConvention();
            Template.RegisterFilter(typeof (Filters));

            _fcgiApplication = new FCGIApplication();
            _fcgiApplication.OnRequestReceived += FcgiApplicationOnOnRequestReceived;
        }

        public void RegisterMiddleware(IMiddleware middleware)
        {
            _beforeView.Add(middleware);
            _afterView.Add(middleware);
        }

        private void FcgiApplicationOnOnRequestReceived(object sender, FastCGI.Request rawRequest)
        {
            var request = new Request(rawRequest);
            try
            {
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
                        Console.WriteLine("{0} {1} {2}", request.Method, httpResponse.StatusCode, request.Path);
                        Console.ResetColor();
                    }
                    response.Write(request);
                }
                else
                    throw new Http404Exception("No route found");
            }
            catch (HttpErrorException e)
            {
                var response = new TemplatedResponse("error", new
                {
                    Title = "Server error",
                    ErrorCode = e.StatusCode,
                    ErrorMessage = e.ToString()
                }, e.StatusCode);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1} {2} {3}", request.Method, e.StatusCode, request.Path, e.Message);
                Console.ResetColor();
                response.Write(request);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1} {2} {3}", request.Method, 500, request.Path, e.Message);
                Console.ResetColor();

                var response = new TemplatedResponse("error", new
                {
                    Title = "Server error",
                    ErrorCode = 500,
                    ErrorMessage = e.ToString()
                }, 500);
                response.Write(request);
            }

            try
            {
                rawRequest.Close();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1} {2}", request.Method, request.Path, e.Message);
                Console.ResetColor();
            }
        }

        public void Listen(int port)
        {
            _fcgiApplication.Run(port);
        }
    }
}