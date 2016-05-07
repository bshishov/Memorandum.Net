using System;
using System.Collections.Generic;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Actions
{
    class FileRawAction : IItemAction<IFileItem>
    {
        public string Editor => "file";
        public string Action => "raw";

        public bool CanHandle(IFileItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            return new StreamedHttpResponse(item.GetStream(), contenttype: item.Mime,
                headers: new Dictionary<string, string>
                {
                    {"Content-Disposition", $"inline; filename=\"{Uri.EscapeDataString(item.Name)}\""},
                    {"X-Sendfile", Uri.EscapeDataString(item.Name)}
                });
        }
    }
}