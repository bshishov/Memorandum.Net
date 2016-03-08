using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Properties;
using Memorandum.Web.Utitlities;
using Memorandum.Web.Views.Drops;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web.Views.Providers
{
    class UrlProvider : BaseProviderView
    {
        public override string Id => "url";

        public override Response NodeView(IRequest request, Node node)
        {
            var urlNode = node as URLNode;
            if(urlNode == null)
                throw new InvalidOperationException("Node is not an URLNode");

            return new TemplatedResponse("url_node", new
            {
                Title = urlNode.Name,
                Node = new UrlNodeDrop(urlNode),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        public override Response NodeAction(IRequest request, Node node, string action)
        {
            var urlNode = node as URLNode;
            if (urlNode == null)
                throw new InvalidOperationException("Node is not an URLNode");

            if (action.Equals("download"))
            {
                using (var info = new UrlInfoParser(urlNode.URL))
                {
                    var dir = Path.Combine(Settings.Default.FileStorage, request.User.Username);
                    Directory.CreateDirectory(dir);
                    var path = Path.Combine(dir, info.FileName);
                    info.SaveContent(path);
                    var fileNode = new FileNode(path);
                    request.UnitOfWork.Files.Save(fileNode);
                    Utilities.CreateLinkForNode(request, urlNode, fileNode, "Downloaded");
                }
                return new RedirectResponse("/url/" + urlNode.Id);
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
            
            if (string.IsNullOrEmpty(request.PostArgs["url"]))
                return new BadRequestApiResponse("Url is not specified");

            using (var info = new UrlInfoParser(request.PostArgs["url"]))
            {
                string imageFileName = null;
                if (!string.IsNullOrEmpty(info.ImageUrl))
                {
                    var dir = Path.Combine(Settings.Default.FileStorage, "thumbnails");
                    Directory.CreateDirectory(dir);
                    imageFileName = Path.GetRandomFileName() + info.ImageFileName;
                    info.SaveImage(Path.Combine(dir, imageFileName));
                }

                var newNode = new URLNode
                {
                    DateAdded = DateTime.Now,
                    URL = request.PostArgs["url"],
                    User = request.User,
                    Name = info.Title,
                };

                if (!string.IsNullOrEmpty(imageFileName))
                    newNode.Image = "/media/thumbnails/" + imageFileName;

                request.UnitOfWork.URL.Save(newNode);
                results.Add(new NodeWithRenderedLink(newNode,
                    Utilities.CreateLinkForNode(request, parentNode, newNode)));

                if (!string.IsNullOrEmpty(request.PostArgs["download"]))
                {
                    var dir = Path.Combine(Settings.Default.FileStorage, request.User.Username);
                    Directory.CreateDirectory(dir);
                    var fileNode = request.UnitOfWork.Files.CreateFileFromStream(Path.Combine(dir, info.FileName), info.ContentStream);
                    Utilities.CreateLinkForNode(request, newNode, fileNode);
                }
            }

            return new ApiResponse(results, 201, "Nodes added");
        }

        public static string GetKnownExtension(URLNode node)
        {
            var uri = new Uri(node.URL);
            var ext = Path.GetExtension(uri.LocalPath);
            return FileProvider.KnownExtensions.Contains(ext) ? ext?.Substring(1) : "_page";
        }
    }
}
