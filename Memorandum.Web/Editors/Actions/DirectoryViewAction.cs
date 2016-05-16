using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Creators;
using Memorandum.Web.ViewModels;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
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
            var baseDrop = parent == null ? null : new DirectoryViewModel(parent);
            return new TemplatedResponse("dir", new
            {
                Title = item.Name,
                BaseDirectory = baseDrop,
                Item = new DirectoryViewModel(item),
                User = new UserViewModel(user),
                Creators = CreatorManager.Creators.Select(c => new CreatorViewModel(c)).ToList()
            });
        }
    }
}
