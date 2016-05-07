using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Actions
{
    class UrlFileViewAction : FileBaseViewAction
    {
        public override string Editor => "url";
        public override string Template => "Files/url";

        public UrlFileViewAction() : base(".url")
        {
        }

        protected override FileItemDrop GetItemView(IRequest request, User user, IFileItem item)
        {
            // TODO: FACTORY OF FILE ITEMS
            var urlItem = new UrlFileItem(item.Owner, item.RelativePath);
            urlItem.Load();
            return new UrlFileItemDrop(urlItem);
        }
    }
}