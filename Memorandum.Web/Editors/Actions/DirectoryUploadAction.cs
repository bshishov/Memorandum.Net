using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class DirectoryUploadAction : IItemAction<IDirectoryItem>
    {
        public string Action => "upload";

        public Response Do(IRequest request, User user, IDirectoryItem item)
        {
            // Save passed files
            foreach (var file in request.Files)
            {
                var fileItem = FileManager.GetFile(item.Owner, Path.Combine(item.RelativePath, file.FileName));
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
