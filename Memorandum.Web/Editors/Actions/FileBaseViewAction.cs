using System;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.ViewModels;
using Shine;
using Shine.Middleware.CSRF;
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

        public IResponse Do(IRequest request, User user, IFileItem item)
        {
            if (!user.CanRead(item))
                throw new InvalidOperationException("You don't have permission to view this item");

            return new TemplatedResponse(Template, new
            {
                Title = item.Name,
                BaseDirectory = new DirectoryViewModel(item.GetParent()),
                Item = _viewFactory.Create(item),
                User = new UserViewModel(user),
                CsrfToken = CsrfMiddleware.GetToken(request)
            });
        }
    }
}
