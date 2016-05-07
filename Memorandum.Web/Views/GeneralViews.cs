using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Actions;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Middleware;

namespace Memorandum.Web.Views
{
    internal static class GeneralViews
    {
        public static Response Home(IRequest request)
        {
            if (((CustomSessionContext)request.Session).User != null)
            {
                return new RedirectResponse("/tree/" + ((CustomSessionContext)request.Session).User.Name + "/");
            }

            return new RedirectResponse("/login");
        }

        public static Response Login(IRequest request)
        {
            if (request.Method == "GET")
            {
                if (((CustomSessionContext)request.Session).User != null)
                    return new RedirectResponse("/");

                return new TemplatedResponse("login", new
                {
                    Title = "Login"
                });
            }

            if (request.Method == "POST")
            {
                var user = UserManager.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user != null)
                {
                    ((CustomSessionContext)request.Session).User = user;
                }
            }

            return new RedirectResponse("/");
        }

        public static Response Logout(IRequest request)
        {
            if (((CustomSessionContext)request.Session).User != null)
            {
                ((CustomSessionContext)request.Session).User = null;
                return new RedirectResponse("/");
            }

            return new RedirectResponse("/");
        }

        private static Response TreeView(IRequest request, string[] args)
        {
            var user = ((CustomSessionContext) request.Session).User;
            if (user == null)
                return new RedirectResponse("/login");

            if (string.IsNullOrEmpty(args[0]))
                throw new InvalidOperationException("Missing args");

            if (((CustomSessionContext)request.Session).User == null)
                throw new InvalidOperationException("Unauthorized");

            var path = string.IsNullOrEmpty(args[1]) ? String.Empty : args[1];
            var item = FileManager.Get(UserManager.Get(args[0]), WebUtility.UrlDecode(path));

            var action = "view"; // default action
            if (!string.IsNullOrEmpty(request.QuerySet["action"]))
                action = request.QuerySet["action"];

            var editor = String.Empty; // default action
            if (!string.IsNullOrEmpty(request.QuerySet["editor"]))
                editor = request.QuerySet["editor"];

            if (item == null)
                throw new Http404Exception("Item not found");

            if (!PermissionManager.CanRead(item, user))
                throw new InvalidOperationException("Access denied :)");

            
            if (item.IsDirectory)
            {
                var dir = item as IDirectoryItem;
                IItemAction<IDirectoryItem> actionImpl;
                if(string.IsNullOrEmpty(editor))
                    actionImpl = DirectoryActions.LastOrDefault(a => a.Action.Equals(action) && a.CanHandle(dir));
                else
                    actionImpl = DirectoryActions.LastOrDefault(a => a.Action.Equals(action) && a.Editor.Equals(editor));

                if (actionImpl == null)
                    throw new InvalidOperationException($"Cannot find implementation of action '{action}'");

                return actionImpl.Do(request, user, dir);
            }
            else
            {
                var file = item as IFileItem;
                IItemAction<IFileItem> actionImpl;
                if (string.IsNullOrEmpty(editor))
                    actionImpl = FileActions.LastOrDefault(a => a.Action.Equals(action) && a.CanHandle(file));
                else
                    actionImpl = FileActions.LastOrDefault(a => a.Action.Equals(action) && a.Editor.Equals(editor));

                if (actionImpl == null)
                    throw new InvalidOperationException($"Cannot find implementation of action '{action}'");

                return actionImpl.Do(request, user, file);
            }
        }
     
        public static Router Router = new Router(new List<IRoute>
        {
            new Route("^/?$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout),
            new RouteWithArg("/tree/([a-z]+)/([^?]+)?", TreeView),
        });

        // TODO: collect using attributes
        private static readonly List<IItemAction<IFileItem>> FileActions = new List<IItemAction<IFileItem>>
        {
            new BinaryFileViewAction(),
            new FileDownloadAction(),
            new FileRawAction(),
            new ItemRenameAction(),
            new CodeFileViewAction(),
            new UrlFileViewAction(),
            new MdFileViewAction(),
        };

        // TODO: collect using attributes
        private static readonly List<IItemAction<IDirectoryItem>> DirectoryActions = new List<IItemAction<IDirectoryItem>>
        {
            new ItemRenameAction(),
            new DirectoryCreateFileAction(),
            new DirectoryViewAction(),
            new DirectoryUploadAction() // duplicate of create ?
        };
    }
}