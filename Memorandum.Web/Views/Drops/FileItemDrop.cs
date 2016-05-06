using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.Views.Drops
{
    public class FileItemDrop : ItemDrop
    {
        public long Size => ((IFileItem) Item).Size;
        public string Mime => ((IFileItem)Item).Mime;


        public FileItemDrop(IFileItem item)
            : base(item)
        {
        }
    }
}