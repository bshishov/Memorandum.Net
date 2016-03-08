using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Views.Drops;
using Memorandum.Web.Views.RestApi;
using Utilities = Memorandum.Web.Utitlities.Utilities;

namespace Memorandum.Web.Views.Providers
{
    class TextNodeProvider : BaseProviderView
    {
        public override string Id => "text";
        public override Response NodeView(IRequest request, Node node)
        {
            var textNode = node as TextNode;
            if (textNode == null)
                throw new InvalidOperationException("Node is not a TextNode");

            return new TemplatedResponse("text_node", new
            {
                Title = "Home",
                Node = new TextNodeDrop(textNode),
                Links = Utilities.GetGroupedLinks(request.UnitOfWork, node)
            });
        }

        public override ApiResponse ApiNodeViewPut(IRequest request, Node node)
        {
            if (request.PostArgs == null)
                return new BadRequestApiResponse("No arguments");

            if (string.IsNullOrEmpty(request.PostArgs["text"]))
                return new BadRequestApiResponse();

            ((TextNode)node).Text = request.PostArgs["text"];
            request.UnitOfWork.Nodes.Save(node);
            return new ApiResponse(NodeDropFactory.Create(node), statusMessage: "Saved");
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
            if (string.IsNullOrEmpty(request.PostArgs["text"]))
                return new BadRequestApiResponse("Text is not specified");

            var newNode = new TextNode
            {
                DateAdded = DateTime.Now,
                Text = request.PostArgs["text"],
                User = request.User
            };

            request.UnitOfWork.Text.Save(newNode);
            results.Add(new NodeWithRenderedLink(newNode,
                Utilities.CreateLinkForNode(request, parentNode, newNode)));

            return new ApiResponse(results, 201, "Nodes added");
        }

        public override Response NodeAction(IRequest request, Node node, string action)
        {
            var textNode = node as TextNode;
            if(textNode == null)
                throw new InvalidOperationException("Node is not a TextNode");

            var arialUni = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            //var bf = BaseFont.CreateFont(arialUni, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            if (action.Equals("pdf"))
            {
                FontFactory.Register(arialUni, "Arial");
                using (var ms = new MemoryStream())
                using (var doc = new Document(PageSize.A4))
                using (var writer = PdfWriter.GetInstance(doc, ms))
                {
                    writer.InitialLeading = 12f;
                    doc.Open();

                    using (var htmlWorker = new HTMLWorker(doc))
                    using (var sr = new StringReader(textNode.Text))
                    {
                        htmlWorker.Parse(sr);
                    }
                    
                    doc.Close();
                    return new HttpResponse(ms.ToArray(), contenttype: "application/pdf");
                }
            }
            return base.ApiNodeAction(request, node, action);
        }
    }
}
