using System;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class ItemDeleteAction : IItemAction<IItem>
    {
        public string Action => "delete";

        public bool CanHandle(IItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IItem item)
        {
            if (!user.Equals(item.Owner))
                throw new InvalidOperationException("You don't have permission to delete this item");

            var dir = item as IDirectoryItem;
            if(dir != null && dir.IsRoot)
                throw new InvalidOperationException("Cannot delete root folder");

            var parent = item.GetParent();
            item.Delete();
            return new RedirectResponse($"/tree/{parent.Owner.Name}/{parent.RelativePath}");
        }
    }
}
