using System;
using System.Collections.Generic;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class FileDownloadAction : IItemAction<IFileItem>
    {
        public string Action => "download";

        public bool CanHandle(IFileItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            if (!user.CanRead(item))
                throw new InvalidOperationException("You don't have permission to download this item");

            return new StreamedHttpResponse(item.GetStream(), contenttype: "application/force-download",
                  headers: new Dictionary<string, string>
                  {
                        {"Content-Disposition", $"attachment; filename=\"{Uri.EscapeDataString(item.Name)}\""},
                        {"X-Sendfile", Uri.EscapeDataString(item.Name)}
                  });
        }
    }
}