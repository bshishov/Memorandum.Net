using System;
using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class FileSaveAction : IItemAction<IFileItem>
    {
        public string Action => "save";

        public bool CanHandle(IFileItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            if(!user.CanWrite(item))
                throw new InvalidOperationException("You don't have permission to edit this file");

            if (request.PostArgs["data"] == null)
                throw new InvalidOperationException("Post arg 'data' required");

            using (var writer = new StreamWriter(item.GetStream(FileMode.Open)))
            {
                writer.Write(request.PostArgs["data"]);
            }

            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}
