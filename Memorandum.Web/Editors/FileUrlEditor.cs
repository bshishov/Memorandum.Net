using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors.Actions;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Editors
{
    class FileUrlEditor : FileEditorBase
    {
        class UrlViewFactory : IViewFactory<IFileItem>
        {
            public ItemViewModel Create(IFileItem item)
            {
                var url = "";
                using (var reader = new StreamReader(item.GetStream(FileMode.Open)))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null && (line.StartsWith("URL") || line.StartsWith("Url")))
                        {
                            url = line.Substring(4);
                        }
                    }
                }
                
                return new FileUrlViewModel(item, url);
            }
        }

        class UrlFileViewAction : FileBaseViewAction
        {
            public override string Template => "Files/url";

            public UrlFileViewAction() : base(new UrlViewFactory())
            {
            }
        }

        public override string Name => "url";

        public FileUrlEditor() : base(new UrlViewFactory(), new[] { ".url" }, new UrlFileViewAction())
        {
        }
    }
}