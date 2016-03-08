using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Utitlities;
using Memorandum.Web.Views.Drops;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web.Views.Providers
{
    abstract class BaseProviderView : IProviderView
    {
        public abstract string Id { get; }
        public abstract Response NodeView(IRequest request, Node node);
        public virtual Response NodeAction(IRequest request, Node node, string action)
        {
            throw new System.NotImplementedException();
        }

        public virtual ApiResponse ApiNodeViewGet(IRequest request, Node node)
        {
            return new ApiResponse(NodeDropFactory.Create(node));
        }

        public virtual ApiResponse ApiNodeViewDelete(IRequest request, Node node)
        {
            if (Equals(node.NodeId, new NodeIdentifier("text", request.User.Home.Id)))
                return new ForbiddenApiResponse("Cannot delete own home");

            Utilities.DeleteLinks(request.UnitOfWork, node);
            request.UnitOfWork.Nodes.Delete(node);
            return new ApiResponse(statusMessage: "Node deleted");
        }

        public virtual ApiResponse ApiNodeViewPut(IRequest request, Node node)
        {
            throw new System.NotImplementedException();
        }

        public virtual ApiResponse ApiNodeAction(IRequest request, Node node, string action)
        {
            throw new System.NotImplementedException();
        }

        public virtual ApiResponse ApiProviderViewGet(IRequest request)
        {
            throw new System.NotImplementedException();
        }

        public virtual ApiResponse ApiProviderViewPost(IRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}