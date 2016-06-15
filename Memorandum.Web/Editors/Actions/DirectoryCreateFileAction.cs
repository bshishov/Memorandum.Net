using System;
using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Creators;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class DirectoryCreateFileAction : IItemAction<IDirectoryItem>
    {
        public string Action => "create";

        public Response Do(IRequest request, User user, IDirectoryItem item)
        {
            if (!user.CanWrite(item))
                throw new InvalidOperationException("You don't have permission to create new files in this directory");

            var creator =
                CreatorManager.Creators.FirstOrDefault(
                    c => c.Id.Equals(request.QuerySet["creator"].ToLowerInvariant()));
            if (creator == null)
                throw new InvalidOperationException("Creator with this name not found");
            var newItem = creator.CreateNew(item, request);
            if (newItem == null)
                throw new InvalidOperationException("Failed to create an item");

            return new RedirectResponse($"/tree/{newItem.Owner.Name}/{newItem.RelativePath}");
        }
    }
}