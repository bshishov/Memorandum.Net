using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Memorandum.Core.Domain;
using Memorandum.Core.Search;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Properties;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views.RestApi
{
    internal static class ApiViews
    {
        private static Response Search(IRequest request)
        {
            const string searchQueryKey = "q";

            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var query = request.QuerySet[searchQueryKey];
            if (string.IsNullOrEmpty(query))
                return new BadRequestApiResponse("Empty query");
            var nodes = new List<Node>();
            nodes.AddRange(SearchManager.TextNodeIndex.Search(query));
            nodes.AddRange(SearchManager.UrlNodeIndex.Search(query));
            nodes.AddRange(SearchManager.FileNodeIndex.Search(query));
            if (string.IsNullOrEmpty(request.QuerySet["mode"]))
            {
                var drops = nodes.Select(NodeDropFactory.Create);
                return new ApiResponse(drops);
            }

            var results = nodes.Select(n => new RenderedNodeDrop(n)).ToList();
            return new ApiResponse(results);
        }

        /// <summary>
        ///     TODO: Implement OAuth
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Response Auth(IRequest request)
        {
            if (request.Method == "POST")
            {
                var loggineduser = UserAuth(request, true);
                if (loggineduser == null)
                    return new BadRequestApiResponse("Already authorized");

                if (request.PostArgs["username"] == null || request.PostArgs["password"] == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var user = request.UnitOfWork.Users.Auth(request.PostArgs["username"], request.PostArgs["password"]);
                if (user == null)
                    return new BadRequestApiResponse("Invalid creditentials");
                var token = Guid.NewGuid().ToString();
                Tokens.Add(token, user);
                return new ApiResponse(new {Token = token});
            }

            return new BadRequestApiResponse();
        }

        /// <summary>
        ///     Returns userobject by FastCGIRequest token, if useCommonAuth eq 'true', then common auth (via sessions) would be preferred
        /// </summary>
        /// <param name="request"></param>
        /// <param name="useCommonAuth"></param>
        /// <returns></returns>
        private static User UserAuth(IRequest request, bool useCommonAuth)
        {
            if (useCommonAuth)
            {
                var userId = request.UserId;
                if (userId != null)
                    return request.UnitOfWork.Users.FindById(userId.Value);
            }

            const string tokenQueryStrigKey = "token";
            var token = request.QuerySet[tokenQueryStrigKey];
            if (string.IsNullOrEmpty(token))
                return null;
            if (Tokens.ContainsKey(token))
                return Tokens[token];
            return null;
        }

        private static Response NodeView(IRequest request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User != null && node.User.Id != user.Id)
                return new ForbiddenApiResponse("Access denied");

            if(request.Method == "GET")
                return new ApiResponse(NodeDropFactory.Create(node));

            if (request.Method == "DELETE")
            {
                if (Equals(node.NodeId, new NodeIdentifier("text", user.Home.Id)))
                    return new ForbiddenApiResponse("Cannot delete own home");

                Utilities.DeleteLinks(request.UnitOfWork, node);
                request.UnitOfWork.Nodes.Delete(node);
                return new ApiResponse(statusMessage: "Node deleted");
            }

            if (request.Method == "PUT")
            {
                if (node is TextNode)
                {
                    if(request.PostArgs == null)
                        return new BadRequestApiResponse("No arguments");

                    if (string.IsNullOrEmpty(request.PostArgs["text"]))
                        return new BadRequestApiResponse();

                    ((TextNode)node).Text = request.PostArgs["text"];
                    request.UnitOfWork.Nodes.Save(node);
                    return new ApiResponse(NodeDropFactory.Create(node), statusMessage: "Saved");
                }

                // TODO: implement put for another providers
            }

            return new BadRequestApiResponse();
        }

        private static Response NodeLinksView(IRequest request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User != null && node.User.Id != user.Id)
                return new ForbiddenApiResponse();

            if (!string.IsNullOrEmpty(request.QuerySet["mode"]))
                return new ApiResponse(Utilities.GetRenderedLinkDrops(request.UnitOfWork, node));

            return new ApiResponse(Utilities.GetLinkDrops(request.UnitOfWork, node));
        }

        private static Response LinksView(IRequest request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            if(string.IsNullOrEmpty(args[0]))
                return new BadRequestApiResponse();

            var link = request.UnitOfWork.Links.FindById(Convert.ToInt32(args[0]));

            if (link == null)
                return new ResourceNotFoundApiResponse();

            if (link.User.Id != user.Id)
                return new ForbiddenApiResponse();

            if (request.Method == "DELETE")
            {
                request.UnitOfWork.Links.Delete(link);
                return new ApiResponse(statusMessage: "Link deleted");
            }

            return new BadRequestApiResponse();
        }
        

        private static Response ApiHome(IRequest request)
        {
            // TODO: describe router | pass Router
            return new ApiResponse(new
            {
                NodesCollection = "/:provider (GET, POST)",
                Node = "/:provider/:id (GET, PUT, DELETE)",
                NodeLinks = "/:provider/:id/links (GET, POST, DELETE)"
            });
        }

        private static readonly Dictionary<string, User> Tokens = new Dictionary<string, User>();
        
        private static Response ProviderView(IRequest request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();
            
            if(string.IsNullOrEmpty(args[0]))
                return new BadRequestApiResponse();

            var provider = args[0];

            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(
                    WebUtility.UrlDecode(request.PostArgs["parent_provider"]), 
                    WebUtility.UrlDecode(request.PostArgs["parent_id"]));
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);

                if (parentNode == null)
                    return new BadRequestApiResponse();

                if (parentNode.User.Id != user.Id)
                    return new ForbiddenApiResponse();

                var results = new List<NodeWithRenderedLink>();

                if (provider == "links")
                {
                    var endNodeId = new NodeIdentifier(
                         WebUtility.UrlDecode(request.PostArgs["end_provider"]),
                         WebUtility.UrlDecode(request.PostArgs["end_id"]));
                    var endNode = request.UnitOfWork.Nodes.FindById(endNodeId);

                    if(endNode == null)
                        return new BadRequestApiResponse();

                    results.Add(new NodeWithRenderedLink(endNode,
                        Utilities.CreateLinkForNode(request, parentNode, endNode)));
                }

                if (provider == "text")
                {
                    if (string.IsNullOrEmpty(request.PostArgs["text"]))
                        return new BadRequestApiResponse("Text is not specified");

                    var newNode = new TextNode
                    {
                        DateAdded = DateTime.Now,
                        Text = request.PostArgs["text"],
                        User = user
                    };

                    request.UnitOfWork.Text.Save(newNode);
                    results.Add(new NodeWithRenderedLink(newNode,
                        Utilities.CreateLinkForNode(request, parentNode, newNode)));
                }

                if (provider == "url")
                {
                    if (string.IsNullOrEmpty(request.PostArgs["url"]))
                        return new BadRequestApiResponse("Url is not specified");

                    var name = Utilities.GetWebPageTitle(request.PostArgs["url"]);
                    var newNode = new URLNode
                    {
                        DateAdded = DateTime.Now,
                        URL = request.PostArgs["url"],
                        User = user,
                        Name = name
                    };

                    request.UnitOfWork.URL.Save(newNode);
                    results.Add(new NodeWithRenderedLink(newNode,
                        Utilities.CreateLinkForNode(request, parentNode, newNode)));
                }

                if (provider == "file")
                {
                    if (!request.Files.Any())
                        return new BadRequestApiResponse("No files passed");

                    var dir = Path.Combine(Settings.Default.FileStorage, user.Username);
                    if (!Directory.Exists(dir))
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
                }

                return new ApiResponse(results, 201, "Nodes added");
            }

            return new BadRequestApiResponse();
        }

        public static Router Router = new Router(new List<IRoute>
        {
            new Route("/$", ApiHome),
            new Route("/auth$", Auth),
            new Route("/search$", Search),
            new RouteWithArg("/links/([0-9]+)$", LinksView),
            new RouteWithArg("/([a-z]+)/(.+)/links$", NodeLinksView),
            new RouteWithArg("/([a-z]+)/([^/]+)$", NodeView),
            new RouteWithArg("/([a-z]+)$", ProviderView)
        });
    }
}