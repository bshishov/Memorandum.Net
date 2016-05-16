using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors.Actions;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Editors
{
    class FileImageEditor : FileEditorBase
    {
        class ImageViewFactory : IViewFactory<IFileItem>
        {
            public ItemViewModel Create(IFileItem item)
            {
                return new FileImageViewModel(item);
            }
        }

        class ImageViewAction : FileBaseViewAction
        {
            public override string Template => "Files/image";
        }

        public override string Name => "image";

        private static readonly string[] KnownExtensions = {
            ".jpg", ".bmp", ".png", ".jpeg", ".gif"
        };

        public FileImageEditor() : base(new ImageViewFactory(), KnownExtensions, new ImageViewAction())
        {
        }
    }
}