using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.ViewModels
{
    public class FileItemViewModel : ItemViewModel
    {
        public long Size => ((IFileItem) Item).Size;
        public string Mime => ((IFileItem)Item).Mime;
        public string Extension => ((IFileItem)Item).Extension;


        public FileItemViewModel(IFileItem item)
            : base(item)
        {
        }

        public override string ThumbnailTemplate => "Blocks/Thumbnails/file";
    }
}