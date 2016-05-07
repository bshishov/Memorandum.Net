﻿using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Creators;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Actions
{
    class DirectoryViewAction : IItemAction<IDirectoryItem>
    {
        public string Editor => "directory";
        public string Action => "view";
        

        public bool CanHandle(IDirectoryItem item)
        {
            return item.Exists;
        }

        public Response Do(IRequest request, User user, IDirectoryItem item)
        {
            var parent = item.GetParent();
            var baseDrop = parent == null ? null : new DirectoryItemDrop(parent);
            return new TemplatedResponse("dir", new
            {
                Title = item.Name,
                BaseDirectory = baseDrop,
                Item = new DirectoryItemDrop(item),
                User = new UserDrop(user),
                Creators = CreatorManager.Creators.Select(c => new CreatorDrop(c)).ToList()
            });
        }
    }
}
