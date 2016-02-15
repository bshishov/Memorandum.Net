using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotLiquid;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using Memorandum.Web.Properties;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Views.RestApi
{
    internal static class ApiViews
    {
        private static Response Search(Request request)
        {
            const string searchQueryKey = "q";

            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var query = request.QuerySet[searchQueryKey];
            if (string.IsNullOrEmpty(query))
                return new BadRequestApiResponse("Empty query");
            var nodes = request.UnitOfWork.Nodes.Search(query);
            if (string.IsNullOrEmpty(request.QuerySet["mode"]))
            {
                var drops = nodes.Select(NodeDropFactory.Create);
                return new ApiResponse(drops);
            }

            var tpl = Template.Parse(File.ReadAllText("Templates/Blocks/_link.liquid"));
            var results = (from node in nodes
                let l = new LinkDrop(new Link(node, node), node)
                select new SearchResult
                {
                    Node = NodeDropFactory.Create(node), Rendered = tpl.Render(Hash.FromAnonymousObject(new {link = l}))
                }).ToList();

            return new ApiResponse(results);
        }

        /// <summary>
        ///     TODO: Implement OAuth
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Response Auth(Request request)
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
        ///     Returns userobject by request token, if useCommonAuth eq 'true', then common auth (via sessions) would be preferred
        /// </summary>
        /// <param name="request"></param>
        /// <param name="useCommonAuth"></param>
        /// <returns></returns>
        private static User UserAuth(Request request, bool useCommonAuth)
        {
            if (useCommonAuth)
            {
                var user = request.Session.Get<User>("user");
                if (user != null)
                    return user;
            }

            const string tokenQueryStrigKey = "token";
            var token = request.QuerySet[tokenQueryStrigKey];
            if (string.IsNullOrEmpty(token))
                return null;
            if (Tokens.ContainsKey(token))
                return Tokens[token];
            return null;
        }

        private static Response NodeView(Request request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User.Id != user.Id)
                return new ForbiddenApiResponse("Access denied");

            if(request.Method == "GET")
                return new ApiResponse(NodeDropFactory.Create(node));

            if (request.Method == "DELETE")
            {
                if (node.NodeId.Id == user.Home.NodeId.Id)
                    return new ForbiddenApiResponse("Cannot delete own home");

                Utilities.DeleteLinks(request.UnitOfWork, node);
                request.UnitOfWork.Nodes.Delete(node);
                return new ApiResponse(new { status = "success" });
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
                    return new ApiResponse(NodeDropFactory.Create(node));
                }
            }

            return new BadRequestApiResponse();
        }

        private static Response NodeLinksView(Request request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();

            var node = request.UnitOfWork.Nodes.FindById(new NodeIdentifier(args[0], args[1]));

            if (node == null)
                return new ResourceNotFoundApiResponse();

            if (node.User.Id != user.Id)
                return new ForbiddenApiResponse();

            return new ApiResponse(Utilities.GetLinks(request.UnitOfWork, node));
        }

        private static Response LinksView(Request request, string[] args)
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
                return new ApiResponse(new {status = "deleted"});
            }

            return new BadRequestApiResponse();
        }


        private static Response ApiHome(Request request)
        {
            // TODO: describe router | pass Router
            return new ApiResponse(new
            {
                NodesCollection = "/:provider (GET, POST)",
                Node = "/:provider/:id (GET, PUT, DELETE)",
                NodeLinks = "/:provider/:id/links (GET, POST, DELETE)"
            });
        }

        public class SearchResult
        {
            public NodeDrop Node { get; set; }
            public String Rendered { get; set; }
        }

        private static readonly Dictionary<string, User> Tokens = new Dictionary<string, User>();
        
        private static Response ProviderView(Request request, string[] args)
        {
            var user = UserAuth(request, true);
            if (user == null)
                return new NonAuthorizedApiResponse();
            
            if(string.IsNullOrEmpty(args[0]))
                return new ForbiddenApiResponse();

            var provider = args[0];

            if (request.Method == "POST")
            {
                var parentNodeId = new NodeIdentifier(request.PostArgs["parent_provider"], request.PostArgs["parent_id"]);
                var parentNode = request.UnitOfWork.Nodes.FindById(parentNodeId);
                if (parentNode == null)
                    return new BadRequestApiResponse();

                if (provider == "text")
                {
                    if (string.IsNullOrEmpty(request.PostArgs["text"]))
                        return new BadRequestApiResponse("text is not specified");

                    var newNode = new TextNode
                    {
                        DateAdded = DateTime.Now,
                        Text = request.PostArgs["text"],
                        User = user
                    };

                    request.UnitOfWork.Text.Save(newNode);
                    Utilities.MakeRelationForNewNode(request, parentNode, newNode);
                    return new ApiResponse(NodeDropFactory.Create(newNode), 201);
                }

                if (provider == "url")
                {
                    if (string.IsNullOrEmpty(request.PostArgs["url"]))
                        return new BadRequestApiResponse("url is not specified");

                    var name = Utilities.GetWebPageTitle(request.PostArgs["url"]);
                    var newNode = new URLNode
                    {
                        DateAdded = DateTime.Now,
                        URL = request.PostArgs["url"],
                        User = user,
                        Name = name
                    };

                    request.UnitOfWork.URL.Save(newNode);
                    Utilities.MakeRelationForNewNode(request, parentNode, newNode);
                    return new ApiResponse(NodeDropFactory.Create(newNode), 201);
                }

                if (provider == "file")
                {
                    if (!request.Files.Any())
                        return new BadRequestApiResponse("No files passed");

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
                            return new BadRequestApiResponse(ex.Message);
                        }
                    }
                    return new ApiResponse(new { staus = "Multiple files uploaded"}, 201);
                }
            }

            if (request.Method == "GET")
            {
                throw new NotImplementedException();
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
            new RouteWithArg("/([a-z]+)/([^/.]+)$", NodeView),
            new RouteWithArg("/([a-z]+)$", ProviderView)
        });
    }
}