using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.ViewModels;
using Shine;
using Shine.Responses;

namespace Memorandum.Web.Editors.Actions
{
    abstract class FileBaseViewAction : IItemAction<IFileItem>
    {
        public string Action => "view";
        public abstract string Template { get; }

        private readonly IViewFactory<IFileItem> _viewFactory;

        protected FileBaseViewAction(IViewFactory<IFileItem> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        protected FileBaseViewAction() : this(new DefaultFileViewFactory())
        {
        }

        protected virtual FileItemViewModel GetItemView(IRequest request, User user, IFileItem item)
        {
            return new FileItemViewModel(item);
        }

        public Response Do(IRequest request, User user, IFileItem item)
        {
            return new TemplatedResponse(Template, new
            {
                Title = item.Name,
                BaseDirectory = new DirectoryViewModel(item.GetParent()),
                Item = _viewFactory.Create(item),
                User = new UserViewModel(user)
            });
        }
    }
}
