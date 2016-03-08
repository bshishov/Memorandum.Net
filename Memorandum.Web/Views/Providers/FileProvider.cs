using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Properties;
using Memorandum.Web.Utitlities;
using Memorandum.Web.Views.Drops;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web.Views.Providers
{
    class FileProvider : BaseProviderView
    {
        public override string Id => "file";

        public override Response NodeView(IRequest request, Node node)
        {
            var baseFileNode = node as BaseFileNode;
            if (baseFileNode == null)
                throw new InvalidOperationException("Node is not a BaseFileNode");

            if (baseFileNode.IsDirectory)
            {
                return new TemplatedResponse("file_node", new
                {
                    Title = baseFileNode.Name,
                    Node = new DirectoryNodeDrop(baseFileNode),
                    Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
                });
            }

            var template = "file_node";
            var fileNode = node as FileNode;
            if (fileNode == null)
                throw new InvalidOperationException("Incorrect file node");

            if (EditableFileTypes.Contains(fileNode.Mime))
                template = "Files/text";
            else if (fileNode.Mime.Contains("image/"))
                template = "Files/image";
            else if (fileNode.Mime.Contains("video/"))
                template = "Files/video";
            else if (fileNode.Mime.Contains("audio/"))
                template = "Files/audio";

            return new TemplatedResponse(template, new
            {
                Title = baseFileNode.Name,
                Node = new FileNodeDrop(baseFileNode),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        public override Response NodeAction(IRequest request, Node node, string action)
        {
            var baseFileNode = node as BaseFileNode;
            if(baseFileNode == null)
                throw new InvalidOperationException("Node is not a BaseFileNode");

            if (action.Equals("raw"))
            {
                if (baseFileNode.IsDirectory)
                    throw new InvalidOperationException("Not a file");
                var fileNode = node as FileNode;
                if (fileNode == null)
                    throw new InvalidOperationException("Not a file");
                
                return new StreamedHttpResponse(File.OpenRead(baseFileNode.Path), contenttype: fileNode.Mime,
                    headers: new Dictionary<string, string>
                    {
                        {"Content-Disposition", $"inline; filename=\"{Uri.EscapeDataString(fileNode.Name)}\""},
                        {"X-Sendfile", Uri.EscapeDataString(fileNode.Name)}
                    });
            }

            if (action.Equals("download"))
            {
                if (baseFileNode.IsDirectory)
                    throw new InvalidOperationException("Not a file");
                var fileNode = node as FileNode;
                if (fileNode == null)
                    throw new InvalidOperationException("Not a file");

                return new StreamedHttpResponse(File.OpenRead(baseFileNode.Path), contenttype: "application/force-download",
                    headers: new Dictionary<string, string>
                    {
                        {"Content-Disposition", $"attachment; filename=\"{Uri.EscapeDataString(fileNode.Name)}\""},
                        {"X-Sendfile", Uri.EscapeDataString(fileNode.Name)}
                    });
            }

            if (action.Equals("edit"))
            {
                return new TemplatedResponse("Files/text", new
                {
                    Title = baseFileNode.Name,
                    Node = new FileNodeDrop(baseFileNode),
                    Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
                });
            }

            return base.NodeAction(request, node, action);
        }

        public override ApiResponse ApiProviderViewPost(IRequest request)
        {
            var parentNodeId = new NodeIdentifier(
                    WebUtility.UrlDecode(request.PostArgs["parent_provider"]),
                    WebUtility.UrlDecode(request.PostArgs["parent_id"]));
            var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);

            if (parentNode == null)
                return new BadRequestApiResponse();

            if (parentNode.User.Id != request.UserId)
                return new ForbiddenApiResponse();

            var results = new List<NodeWithRenderedLink>();

            if (!request.Files.Any())
                return new BadRequestApiResponse("No files passed");

            var dir = Path.Combine(Settings.Default.FileStorage, request.User.Username);
            Directory.CreateDirectory(dir);

            foreach (var file in request.Files)
            {
                var filePath = Path.Combine(dir, file.FileName);
                try
                {
                    var fileNode = request.UnitOfWork.Files.CreateFileFromStream(filePath, file.Data);
                    results.Add(new NodeWithRenderedLink(fileNode,
                        Utilities.CreateLinkForNode(request, parentNode, fileNode)));
                }
                catch (Exception ex)
                {
                    return new BadRequestApiResponse(ex.Message);
                }
            }

            return new ApiResponse(results, 201, "Nodes added");
        }

        public static string GetKnownExtension(FileNode node)
        {
            var ext = Path.GetExtension(node.Path);
            return KnownExtensions.Contains(ext) ? ext?.Substring(1) : null;
        }

        /// <summary>
        /// Related to /static/img/file-icons
        /// </summary>
        public static readonly List<string> KnownExtensions = new List<string>
        {
            ".aac", ".ai", ".aiff", ".avi",
            ".bmp",
            ".c", ".cpp", ".css",
            ".dat", ".dmg", ".doc", ".dotx", ".docx", ".dwg", ".dxf",
            ".eps", ".exe",
            ".flv",
            ".gif",
            ".h", ".hpp", ".html",
            ".ics", ".iso",
            ".java", ".jpg", ".js",
            ".key",
            ".less",
            ".mid", ".mp3", ".mp4", ".mpg",
            ".odf", ".ods", ".odt", ".otp", ".ots", ".ott",
            ".pdf", ".php", ".png", ".ppt", ".psd", ".py",
            ".qt",
            ".rar", ".rb", ".rtf",
            ".sass", ".scss", ".sql",
            ".tga", ".tgz", ".tiff", ".txt",
            ".wav",
            ".xls", ".xlsx",
            ".xml",
            ".yml",
            ".zip",
        };


        private static readonly string[] EditableFileTypes =
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
