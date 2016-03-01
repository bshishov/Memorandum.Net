using System;
using CommandLine;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Core.Search;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Routing;
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
            var result = Parser.Default.ParseArguments<RunServerOptions, CreateSchemaOptions>(args);
            var exitCode = result.MapResult(
                (RunServerOptions opts) => Runserver(opts),
                (CreateSchemaOptions opts) => CreateSchema(opts),
                errors => 1);
            return exitCode;
        }

        private static int Runserver(RunServerOptions options)
        {
            var router = new Router();
            router.Bind("", GeneralViews.Router);
            router.Bind("^/text", TextNodeViews.Router);
            router.Bind("^/url", UrlNodeViews.Router);
            router.Bind("^/file", FileNodeViews.Router);
            router.Bind("^/api", ApiViews.Router);

            if (options.ForceReindex)
                SearchManager.StartIndexingTask();

            var app = new App(router);
            app.RegisterMiddleware(new UnitOfWorkMiddleware());
            app.RegisterMiddleware(new SessionMiddleware());
            app.Listen(Settings.Default.Port);

            return 0;
        }

        private static int AddUser(CreateUserOptions options)
        {
            using (var unit = new UnitOfWork())
            {
                var password = unit.Users.CreateNewPasswordString(options.Password);
                var user = new User
                {
                    DateJoined = DateTime.Now,
                    Email = options.Email,
                    Password = password,
                    Username = options.Username
                };
                unit.Users.Save(user);
                var node = new TextNode
                {
                    DateAdded = DateTime.Now,
                    Text = "Home",
                    User = user
                };
                unit.Text.Save(node);
                user.Home = node;
                unit.Users.Save(user);
            }

            return 0;
        }

        private static int CreateSchema(CreateSchemaOptions options)
        {
            try
            {
                Database.CreateSchema();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 1;
            }

            return 0;
        }

        [Verb("runserver", HelpText = "Run server")]
        private class RunServerOptions
        {
            [Option("forceindex", Required = false, HelpText = "Forces reindexing")]
            public bool ForceReindex { get; set; }
        }

        [Verb("createschema", HelpText = "Creates database schema")]
        private class CreateSchemaOptions
        {
        }

        [Verb("adduser", HelpText = "Create new user")]
        private class CreateUserOptions
        {
            [Option('u', "username", Required = true, HelpText = "Username")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password")]
            public string Password { get; set; }

            [Option('e', "email", Required = true, HelpText = "Email")]
            public string Email { get; set; }
        }
    }
}