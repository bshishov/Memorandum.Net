using Memorandum.Core.Domain.Files;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Editors
{
    class DefaultFileViewFactory : IViewFactory<IFileItem>
    {
        public ItemViewModel Create(IFileItem item)
        {
            return new FileItemViewModel(item);
        }
    }
}