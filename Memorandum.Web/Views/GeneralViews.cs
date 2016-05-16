﻿using System;
using System.Collections.Generic;
using System.Net;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Editors;
using Memorandum.Web.Middleware;
using Shine;
using Shine.Errors;
using Shine.Responses;
using Shine.Routing;

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

            var editorName = String.Empty; // default action
            if (!string.IsNullOrEmpty(request.QuerySet["editor"]))
                editorName = request.QuerySet["editor"];

            if (item == null)
                throw new Http404Exception("Item not found");

            if (!PermissionManager.CanRead(item, user))
                throw new InvalidOperationException("Access denied :)");

            
            if (item.IsDirectory)
            {
                var dir = item as IDirectoryItem;
                var editor = string.IsNullOrEmpty(editorName) ? 
                    EditorManager.GetDefaultEditor(dir) : EditorManager.GetDirectoryEditor(editorName);

                if (editor == null)
                    throw new InvalidOperationException($"Cannot find implementation of action '{action}'");

                return editor.GetAction(action).Do(request, user, dir);
            }
            else
            {
                var file = item as IFileItem;
                var editor = string.IsNullOrEmpty(editorName) ?
                   EditorManager.GetDefaultEditor(file) : EditorManager.GetFileEditor(editorName);

                if (editor == null)
                    throw new InvalidOperationException($"Cannot find implementation of action '{action}'");

                return editor.GetAction(action).Do(request, user, file);
            }
        }
     
        public static Router Router = new Router(new List<IRoute>
        {
            new Route("^/?$", Home),
            new Route("^/login$", Login),
            new Route("^/logout$", Logout),
            new RouteWithArg("/tree/([a-z]+)/([^?]+)?", TreeView),
        });
    }
}