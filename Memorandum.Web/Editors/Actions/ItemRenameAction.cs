using System;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class ItemRenameAction : IItemAction<IItem>
    {
        public string Action => "rename";

        public bool CanHandle(IItem item)
        {
            return item.Exists;
        }

        public IResponse Do(IRequest request, User user, IItem item)
        {
            if (!user.CanWrite(item))
                throw new InvalidOperationException("You don't have permission to edit this file");

            if (string.IsNullOrEmpty(request.PostArgs["name"])) 
                throw new InvalidOperationException("POST argument 'name' expected");

            item.Rename(request.PostArgs["name"]);
            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}
