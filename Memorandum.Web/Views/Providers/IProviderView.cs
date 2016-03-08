using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Views.RestApi;

namespace Memorandum.Web.Views.Providers
{
    interface IProviderView
    {
        string Id { get; }
        Response NodeView(IRequest request, Node node);
        Response NodeAction(IRequest request, Node node, string action);

        ApiResponse ApiProviderViewGet(IRequest request);
        ApiResponse ApiProviderViewPost(IRequest request);

        ApiResponse ApiNodeViewGet(IRequest request, Node node);
        ApiResponse ApiNodeViewPut(IRequest request, Node node);
        ApiResponse ApiNodeViewDelete(IRequest request, Node node);

        ApiResponse ApiNodeAction(IRequest request, Node node, string action);
    }
}
