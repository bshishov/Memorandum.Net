using System;
using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Creators;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Actions
{
    class DirectoryCreateFileAction : IItemAction<IDirectoryItem>
    {
        public string Editor => "directory";
        public string Action => "create";

        public bool CanHandle(IDirectoryItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IDirectoryItem item)
        {
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