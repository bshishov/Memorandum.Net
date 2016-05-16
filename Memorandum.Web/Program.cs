﻿using System;
using System.IO;
using CommandLine;
using Memorandum.Core.Domain.Users;
using Memorandum.Core.Search;
using Memorandum.Web.Middleware;
using Memorandum.Web.Properties;
using Memorandum.Web.ViewModels;
using Memorandum.Web.Views;
using Memorandum.Web.Views.RestApi;
using Shine;
using Shine.Responses;
using Shine.Routing;
using Shine.Server;
using Shine.Server.HttpListener;
using Shine.Templates.DotLiquid;
using Shine.Utilities;

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
                router.Bind("^/media", new StaticServeRouter(Settings.Default.MediaPath));
            }

            router.Bind("^/api", ApiViews.Router);
            router.Bind(String.Empty, GeneralViews.Router);

            if (options.ForceReindex)
                SearchManager.StartIndexingTask();

            var templateEngine = new DotLiquidTemplateProcessor("../../Templates");
            templateEngine.RegisterSafeType<CreatorViewModel>();
            templateEngine.RegisterSafeType<DirectoryViewModel>();
            templateEngine.RegisterSafeType<FileImageViewModel>();
            templateEngine.RegisterSafeType<FileItemViewModel>();
            templateEngine.RegisterSafeType<ItemViewModel>();
            templateEngine.RegisterSafeType<UserViewModel>();
            templateEngine.RegisterSafeType<FileUrlViewModel>();

            var app = new App(router);
            app.SetTemplateProcessor(templateEngine);
            app.RegisterMiddleware(new CustomSessionMiddleware());
            app.RegisterMiddleware(new ApiMiddleware("/api"));

            app.ErrorHandler = (request, code, e) => new TemplatedResponse("error", new
            {
                Title = "Server error",
                ErrorCode = code,
                ErrorMessage = e.Message,
                Exception = e.ToString()
            }, code);

            /*IServer server;
            
            if (options.FastCGI)
                server = new FastCGIServer(Settings.Default.Port);
            else
                server = new HttpListenerServer($"http://127.0.0.1:{Settings.Default.Port}/");*/

            var server = new HttpListenerServer($"http://127.0.0.1:{Settings.Default.Port}/");
            server.Run(app);
            Console.ReadKey();
            return 0;
        }

        private static int AddUser(CreateUserOptions options)
        {
            var baseDir = options.BaseDirectory;
            if (string.IsNullOrEmpty(baseDir))
                baseDir = Path.Combine(Settings.Default.MediaPath, options.Username);
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

            [Option("fastcgi", Required = false, HelpText = "Use fastcgi server")]
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