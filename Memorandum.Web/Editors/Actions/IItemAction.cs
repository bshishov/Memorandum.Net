using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    interface IItemAction<in T>
        where T : IItem
    {
        string Action { get; }
        Response Do(IRequest request, User user, T item);
    }
}