using Memorandum.Core.Domain.Files;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Editors
{
    public interface IFileEditor
    {
        string Name { get; }
        string Template { get; }
        bool CanHandle(IFileItem item);
        FileItemDrop GetView(IFileItem item);
    }
}
