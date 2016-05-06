using System;
using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Framework;
using Memorandum.Web.Utitlities;

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
            var urlItem = new UrlFileItem(url, directory.Owner, Path.Combine(directory.RelativePath,"some" + DefaultExtension));
            urlItem.Save();
            return urlItem;
        }
    }
}
