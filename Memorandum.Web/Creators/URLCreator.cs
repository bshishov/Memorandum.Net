using System;
using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Utitlities;
using Shine;

namespace Memorandum.Web.Creators
{
    class URLCreator : ICreator
    {
        public string Id => "url";
        public string Name => "URL";
        public string Template => "Blocks/Forms/url";
        public string DefaultExtension => ".url";

        public IItem CreateNew(IDirectoryItem directory, IRequest request)
        {
            var url = request.PostArgs["url"]; // TODO: validate!

            if(url == null)
                throw new InvalidOperationException("Url is not set");

            var urlInfo = new UrlInfoParser(url);

            var item = FileItem.Create(directory.Owner, Path.Combine(directory.RelativePath, $"{urlInfo.FileName}.url"));
            using (var writer = new StreamWriter(item.GetStream(FileMode.OpenOrCreate)))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine($"URL={url}");
            }
            
            return item;
        }
    }
}
