using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Actions
{
    abstract class FileBaseViewAction : IItemAction<IFileItem>
    {
        public abstract string Editor { get; }
        public string Action => "view";
        public abstract string Template { get; }


        private readonly string[] _extensions;

        protected FileBaseViewAction(params string[] extensions)
        {
            _extensions = extensions;
        }

        public bool CanHandle(IFileItem item)
        {
            return item.Exists && (_extensions.Length == 0 || _extensions.Contains(item.Extension));
        }

        protected virtual FileItemDrop GetItemView(IRequest request, User user, IFileItem item)
        {
            return new FileItemDrop(item);
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            return new TemplatedResponse(Template, new
            {
                Title = item.Name,
                BaseDirectory = new DirectoryItemDrop(item.GetParent()),
                Item = GetItemView(request, user, item),
                User = new UserDrop(user)
            });
        }
    }
}
