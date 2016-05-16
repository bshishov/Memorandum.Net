using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.ViewModels
{
    class FileUrlViewModel : FileItemViewModel
    {
        public string TargetUrl { get; }

        public FileUrlViewModel(IFileItem item, string url) : base(item)
        {
            TargetUrl = url;
        }
    }
}
