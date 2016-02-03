using System;
using CommandLine;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Middleware;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web
{
    class Program
    {
        [Verb("runserver", HelpText = "Run server")]
        class RunServerOptions 
        {
        }

        [Verb("createschema", HelpText = "Creates database schema")]
        class CreateSchemaOptions
        {
        }

        [Verb("adduser", HelpText = "Create new user")]
        class CreateUserOptions
        {
            [Option('u', "username", Required = true, HelpText = "Username")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password")]
            public string Password { get; set; }

            [Option('e', "email", Required = true, HelpText = "Email")]
            public string Email { get; set; }
        }


        static int Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<RunServerOptions, CreateSchemaOptions>(args);
            var exitCode = result.MapResult(
                (RunServerOptions opts) => Runserver(opts),
                (CreateSchemaOptions opts) => CreateSchema(opts), 
                errors => 1);
            return exitCode;
        }

        static int Runserver(RunServerOptions options)
        {
            var router = new Router();
            router.Bind("", Views.GeneralViews.Router);
            router.Bind("^/text", Views.TextNodeViews.Router);
            router.Bind("^/url", Views.UrlNodeViews.Router);
            router.Bind("^/file", Views.FileNodeViews.Router);
            router.Bind("^/api", ApiViews.Router);

            var app = new App(router);
            app.RegisterMiddleware(new SessionMiddleware());
            app.RegisterMiddleware(new UnitOfWorkMiddleware());
            app.Listen(Properties.Settings.Default.Port);
            return 0;
        }

        static int AddUser(CreateUserOptions options)
        {
            using (var unit = new UnitOfWork())
            {
                var password = unit.Users.CreateNewPasswordString(options.Password);
                var user = new User()
                {
                    DateJoined = DateTime.Now,
                    Email = options.Email,
                    Password = password,
                    Username = options.Username
                };
                unit.Users.Save(user);
                var node = new TextNode()
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

        static int CreateSchema(CreateSchemaOptions options)
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
    }
}
