using System;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class ItemShareAction : IItemAction<IItem>
    {
        public string Action => "share";

        public bool CanHandle(IItem item)
        {
            return item.Exists;
        }

        public IResponse Do(IRequest request, User user, IItem item)
        {
            if (!user.Equals(item.Owner))
                throw new InvalidOperationException("Only owner of this item can share it");

            if (request.PostArgs["target"] == null)
                throw new ArgumentException("Target must be specified");

            if (request.PostArgs["type"] == null)
                throw new ArgumentException("Sharing type must be specified");

            var targetUser = UserManager.Get(request.PostArgs["target"]);
            if(targetUser == null)
                throw new ArgumentException("Can't find user with specified name");

            SharingType sharingType;
            switch (request.PostArgs["type"])
            {
                case "read":
                    sharingType = SharingType.ReadOnly;
                    break;
                case "write":
                    sharingType = SharingType.ReadAndWrite;
                    break;
                case "forbidden":
                    sharingType = SharingType.Forbidden;
                    break;
                default:
                    throw new ArgumentException("Specified sharing type is incorrect");
            }

            user.Share(item, targetUser, sharingType);
            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}