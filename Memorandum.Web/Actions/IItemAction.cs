using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Actions
{
    interface IItemAction<in T>
        where T : IItem
    {
        string Editor { get; }
        string Action { get; }
        bool CanHandle(T item);
        Response Do(IRequest request, User user, T item);
    }
}