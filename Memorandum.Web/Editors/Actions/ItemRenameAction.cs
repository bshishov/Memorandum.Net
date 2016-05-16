﻿using System;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    class ItemRenameAction : IItemAction<IItem>
    {
        public string Editor => "item";
        public string Action => "rename";

        public bool CanHandle(IItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IItem item)
        {
            if (string.IsNullOrEmpty(request.PostArgs["name"])) 
                throw new InvalidOperationException("POST argument 'name' expected");
                

            item.Rename(request.PostArgs["name"]);
            return new RedirectResponse($"/tree/{item.Owner.Name}/{item.RelativePath}");
        }
    }
}