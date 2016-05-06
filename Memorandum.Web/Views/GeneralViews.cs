using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Editors;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Middleware;
using Memorandum.Web.Views.Drops;

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
            var action = request.QuerySet["action"]?.ToLowerInvariant();

            if (item == null)
                throw new Http404Exception("Item not found");

            if (!PermissionManager.CanRead(item, user))
                throw new InvalidOperationException("Access denied :)");

            if (request.Method == "POST" && !string.IsNullOrEmpty(action))
            {
                if (action.Equals("rename"))
                {
                    item.Rename(request.PostArgs["name"]);
                    return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
                }
            }

            if (item.IsDirectory)
            {
                var dir = (IDirectoryItem)item;

                if (request.Method == "POST" && !string.IsNullOrEmpty(action))
                {
                    if (action.Equals("create"))
                    {
                        var creator =
                        EditorManager.Creators.FirstOrDefault(
                            c => c.Id.Equals(request.QuerySet["creator"].ToLowerInvariant()));
                        if (creator == null)
                            throw new InvalidOperationException("Creator with this name not found");
                        var newItem = creator.CreateNew(dir, request);
                        if (newItem == null)
                            throw new InvalidOperationException("Failed to create an item");
                        return new RedirectResponse($"/tree/{newItem.Owner.Name}/{newItem.RelativePath}");
                    }
                }

                var parent = dir.GetParent();
                var baseDrop = parent == null ? null : new DirectoryItemDrop(parent);
                return new TemplatedResponse("dir", new
                {
                    Title = item.Name,
                    BaseDirectory = baseDrop,
                    Item = new DirectoryItemDrop(dir),
                    User = new UserDrop(user),
                    Creators = EditorManager.Creators.Select(c => new CreatorDrop(c)).ToList()
                });
            }

            var fileItem = item as IFileItem;
            if (fileItem == null)
                throw new InvalidOperationException("Incorrect file Item");

            IFileEditor fileEditor;
            if (!string.IsNullOrEmpty(request.QuerySet["editor"]))
                fileEditor = EditorManager.GetEditor(request.QuerySet["editor"]);
            else
                fileEditor = EditorManager.GetEditor(fileItem);

            return new TemplatedResponse(fileEditor.Template, new
            {
                Title = item.Name,
                BaseDirectory = new DirectoryItemDrop(fileItem.GetParent()),
                Item = fileEditor.GetView(fileItem),
                User = new UserDrop(user)
            });
        }

        private static Response TreeViewAction(IRequest request, string[] args)
        {
            if (((CustomSessionContext)request.Session).User == null)
                return new RedirectResponse("/login");

            if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
                throw new InvalidOperationException("Missing args");

            if (((CustomSessionContext)request.Session).User == null)
                throw new InvalidOperationException("Unauthorized");

            var node = FileManager.Get(UserManager.Get(args[1]), args[2]);

            if (node == null)
                throw new Http404Exception("Item not found");

            if (!PermissionManager.CanRead(node, ((CustomSessionContext)request.Session).User))
                throw new InvalidOperationException("Access denied :)");

            if (args[0].Equals("raw") && request.Method == "GET")
            {
                var fileNode = node as IFileItem;
                if (fileNode == null)
                    throw new InvalidOperationException("Not a file");

                return new StreamedHttpResponse(fileNode.GetStream(), contenttype: fileNode.Mime,
                    headers: new Dictionary<string, string>
                    {
                        {"Content-Disposition", $"inline; filename=\"{Uri.EscapeDataString(fileNode.Name)}\""},
                        {"X-Sendfile", Uri.EscapeDataString(fileNode.Name)}
                    });
            }

            if (args[0].Equals("download") && request.Method == "GET")
            {
                var fileNode = node as IFileItem;
                if (fileNode == null)
                    throw new InvalidOperationException("Not a file");

                return new StreamedHttpResponse(fileNode.GetStream(), contenttype: "application/force-download",
                    headers: new Dictionary<string, string>
                    {
                        {"Content-Disposition", $"attachment; filename=\"{Uri.EscapeDataString(fileNode.Name)}\""},
                        {"X-Sendfile", Uri.EscapeDataString(fileNode.Name)}
                    });
            }

            throw new NotImplementedException();
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("^/?$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout),
            new RouteWithArg("/tree/([a-z]+)/([^?]+)?", TreeView),
            new RouteWithArg("/([a-z]+)/([a-z]+)/([^?]+)?", TreeViewAction),
        });
    }
}