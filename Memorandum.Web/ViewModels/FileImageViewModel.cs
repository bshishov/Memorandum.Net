using System;
using System.Drawing;
using System.Drawing.Imaging;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Properties;

namespace Memorandum.Web.ViewModels
{
    class FileImageViewModel : FileItemViewModel
    {
        private static readonly string ThumbnailsDirectory 
            = System.IO.Path.Combine(Settings.Default.MediaPath, "thumbnails");
        private readonly string _thumbnail;

        public string ThumbnailPath
        {
            get
            {
                if(!System.IO.File.Exists(System.IO.Path.Combine(ThumbnailsDirectory, _thumbnail)))
                    CreatePreview();
                return _thumbnail;
            }
        }

        public FileImageViewModel(IFileItem item) : base(item)
        {
            _thumbnail = $"{item.GetHash()}.png";
        }

        private void CreatePreview()
        {
            var src = Image.FromFile(Item.FileSystemPath);
            var thumb = src.GetThumbnailImage(200, 200, null, IntPtr.Zero);

            if (!System.IO.Directory.Exists(ThumbnailsDirectory))
                System.IO.Directory.CreateDirectory(ThumbnailsDirectory);
            thumb.Save(System.IO.Path.Combine(ThumbnailsDirectory, _thumbnail), ImageFormat.Png);
        }
    }
}
