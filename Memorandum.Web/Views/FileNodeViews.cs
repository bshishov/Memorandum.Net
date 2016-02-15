using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Errors;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Properties;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views
{
    internal static class FileNodeViews
    {
        private static Response FileNodeAdd(Request request)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");
            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(request.PostArgs["parent_provider"], request.PostArgs["parent_id"]);
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);
                if (parentNode == null || !request.Files.Any())
                    throw new Http500Exception("Incorect parameters");

                var dir = Path.Combine(Settings.Default.FileStorage, user.Username);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                foreach (var file in request.Files)
                {
                    var filePath = Path.Combine(dir, file.FileName);
                    try
                    {
                        using (var fileStream = File.Create(filePath))
                        {
                            file.Data.Seek(0, SeekOrigin.Begin);
                            file.Data.CopyTo(fileStream);
                        }
                        var fileNode = new FileNode(filePath);
                        //request.UnitOfWork.Files.Save(fileNode);
                        Utilities.MakeRelationForNewNode(request, parentNode, fileNode);
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Http500Exception(ex);
                    }
                }

                return new RedirectResponse("/" + parentNode.NodeId.Provider + "/" + parentNode.NodeId.Id);
            }

            throw new Http404Exception("POST expected");
        }

        private static Response FileNodeView(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.Files.FindById(WebUtility.UrlDecode(args[0]));
            if (node.IsDirectory)
            {
                return new TemplatedResponse("_file_node", new
                {
                    Title = node.Name,
                    Node = new DirectoryNodeDrop(node),
                    Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
                });
            }

            var template = "_file_node";
            var fileNode = node as FileNode;
            if (fileNode == null)
                throw new Exception("Incorrect file node");

            if (EditableFileTypes.Contains(fileNode.Mime))
                template = "FileNodes/_text";
            else if (fileNode.Mime.Contains("image/"))
                template = "FileNodes/_image";
            else if (fileNode.Mime.Contains("video/"))
                template = "FileNodes/_video";
            else if (fileNode.Mime.Contains("audio/"))
                template = "FileNodes/_audio";

            return new TemplatedResponse(template, new
            {
                Title = node.Name,
                Node = new FileNodeDrop(node),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        private static Response RawFileNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.Files.FindById(args[0]);
            if (node.IsDirectory)
                throw new InvalidOperationException("Not a file");
            var fileNode = node as FileNode;
            if (fileNode == null)
                throw new InvalidOperationException("Not a file");
            return new HttpResponse(File.ReadAllBytes(node.Path), contenttype: fileNode.Mime);
        }

        private static Response DownloadFileNode(Request request, string[] args)
        {
            var user = request.Session.Get<User>("user");
            if (user == null)
                return new RedirectResponse("/login");

            var node = request.UnitOfWork.Files.FindById(args[0]);
            if (node.IsDirectory)
                throw new InvalidOperationException("Not a file");
            var fileNode = node as FileNode;
            if (fileNode == null)
                throw new InvalidOperationException("Not a file");

            return new HttpResponse(File.ReadAllBytes(node.Path), contenttype: "application/force-download",
                attributes: new Dictionary<string, string>
                {
                    {"Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileNode.Name)},
                    {"X-Sendfile", fileNode.Name}
                });
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("/add$", FileNodeAdd),
            new RouteWithArg("/raw/(.+)$", RawFileNode),
            new RouteWithArg("/download/(.+)$", DownloadFileNode),
            new RouteWithArg("/(.+)$", FileNodeView)
        });

        public static string[] EditableFileTypes =
        {
            "application/javascript",
            "application/json",
            "application/atom+xml",
            "application/rss+xml",
            "application/xml",
            "application/sparql-query",
            "application/sparql-results+xml",
            "text/calendar",
            "text/css",
            "text/csv",
            "text/html",
            "text/n3",
            "text/plain",
            "text/plain-bas",
            "text/prs.lines.tag",
            "text/richtext",
            "text/sgml",
            "text/tab-separated-values",
            "text/troff",
            "text/turtle",
            "text/uri-list",
            "text/vnd.curl",
            "text/vnd.curl.dcurl",
            "text/vnd.curl.mcurl",
            "text/vnd.curl.scurl",
            "text/vnd.fly",
            "text/vnd.fmi.flexstor",
            "text/vnd.graphviz",
            "text/vnd.in3d.3dml",
            "text/vnd.in3d.spot",
            "text/vnd.sun.j2me.app-descriptor",
            "text/vnd.wap.wml",
            "text/vnd.wap.wmlscript",
            "text/x-asm",
            "text/x-c",
            "text/x-fortran",
            "text/x-java-source,java",
            "text/x-pascal",
            "text/x-setext",
            "text/x-uuencode",
            "text/x-vcalendar",
            "text/x-vcard",
            "text/yaml"
        };
    }
}