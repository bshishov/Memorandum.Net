using Memorandum.Core.Domain.Files;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Editors
{
    class UrlEditor : FileEditor
    {
        public override string Name => "url";
        public override string Template => "Files/url";

        public UrlEditor() : base(".url")
        {
        }

        public override FileItemDrop GetView(IFileItem item)
        {
            // TODO: FACTORY OF FILE ITEMS
            var urlItem = new UrlFileItem(item.Owner, item.RelativePath);
            urlItem.Load();
            return new UrlFileItemDrop(urlItem);
        }
    }
}
