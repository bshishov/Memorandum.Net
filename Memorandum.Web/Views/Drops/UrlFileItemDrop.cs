using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.Views.Drops
{
    class UrlFileItemDrop : FileItemDrop
    {
        public string TargetUrl { get; }

        public UrlFileItemDrop(UrlFileItem item) : base(item)
        {
            TargetUrl = item.Url;
        }
    }
}
