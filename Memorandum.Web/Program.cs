using System;
using System.IO;
using CommandLine;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Users;
using Memorandum.Core.Search;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Backend;
using Memorandum.Web.Framework.Backend.FastCGI;
using Memorandum.Web.Framework.Backend.HttpListener;
using Memorandum.Web.Framework.Middleware.Session;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Middleware;
using Memorandum.Web.Properties;
using Memorandum.Web.Views;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<RunServerOptions, CreateUserOptions> (args);
            var exitCode = result.MapResult(
                (RunServerOptions opts) => Runserver(opts),
                (CreateUserOptions opts) => AddUser(opts),
                errors => 1);
            return exitCode;
        }

        private static int Runserver(RunServerOptions options)
        {
            var router = new Router();

            if (Settings.Default.ServeStatic)
            {
                router.Bind("^/static", new StaticServeRouter(Settings.Default.StaticPath));
                router.Bind("^/media", new StaticServeRouter(Settings.Default.FileStorage));
            }

            router.Bind("^/api", ApiViews.Router);
            router.Bind("", GeneralViews.Router);

            if (options.ForceReindex)
                SearchManager.StartIndexingTask();

            IBackend backend;
            if (options.FastCGI)
                backend = new FastCGIBackend(Settings.Default.Port);
            else
                backend = new HttpListenerBackend($"http://127.0.0.1:{Settings.Default.Port}/");

            var app = new App(backend, router);
            app.RegisterMiddleware(new CustomSessionMiddleware(new MemorySessionStorage()));
            app.RegisterMiddleware(new ApiMiddleware("/api"));
            app.Run();
            Console.ReadKey();
            return 0;
        }

        private static int AddUser(CreateUserOptions options)
        {
            var baseDir = options.BaseDirectory;
            if (string.IsNullOrEmpty(baseDir))
                baseDir = Path.Combine(Settings.Default.FileStorage, options.Username);
            var user = UserManager.Create(options.Username, options.Password, baseDir);
            if(user != null)
                return 0;
            return 1;
        }
      

        [Verb("runserver", HelpText = "Run server")]
        private class RunServerOptions
        {
            [Option("forceindex", Required = false, HelpText = "Forces reindexing")]
            public bool ForceReindex { get; set; }

            [Option("fastcgi", Required = false, HelpText = "Use fastcgi backend")]
            public bool FastCGI { get; set; }
        }
     

        [Verb("adduser", HelpText = "Create new user")]
        private class CreateUserOptions
        {
            [Option('u', "name", Required = true, HelpText = "Name")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password")]
            public string Password { get; set; }

            [Option('b', "base", Required = false, HelpText = "Base Directory")]
            public string BaseDirectory { get; set; }
        }
    }
}