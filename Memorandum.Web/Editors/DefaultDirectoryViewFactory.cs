using Memorandum.Core.Domain.Files;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Editors
{
    class DefaultDirectoryViewFactory : IViewFactory<IDirectoryItem>
    {
        public ItemViewModel Create(IDirectoryItem item)
        {
            return new DirectoryViewModel(item);
        }
    }
}