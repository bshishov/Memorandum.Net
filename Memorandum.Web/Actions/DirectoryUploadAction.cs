using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Actions
{
    class DirectoryUploadAction : IItemAction<IDirectoryItem>
    {
        public string Editor => "directory";
        public string Action => "upload";

        public bool CanHandle(IDirectoryItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IDirectoryItem item)
        {
            // Save passed files
            foreach (var file in request.Files)
            {
                var fileItem = new FileItem(item.Owner, Path.Combine(item.RelativePath, file.FileName));
                // if(fileItem.Exists) // TODO: overwrite ?
                using (var writer = fileItem.GetStream(FileMode.OpenOrCreate))
                {
                    file.Data.CopyTo(writer);
                }
            }

            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}
