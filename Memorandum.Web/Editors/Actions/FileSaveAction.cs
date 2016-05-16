using System;
using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class FileSaveAction : IItemAction<IFileItem>
    {
        public string Editor => "file";
        public string Action => "save";

        public bool CanHandle(IFileItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            if(request.PostArgs["data"] == null)
                throw new InvalidOperationException("Post arg 'data' required");

            using (var writer = new StreamWriter(item.GetStream(FileMode.Open)))
            {
                writer.Write(request.PostArgs["data"]);
            }

            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}
