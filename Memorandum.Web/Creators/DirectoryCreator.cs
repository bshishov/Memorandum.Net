using System;
using System.IO;
using Memorandum.Core.Domain.Files;
using Shine;

namespace Memorandum.Web.Creators
{
    class DirectoryCreator : ICreator
    {
        public string Id => "folder";
        public string Name => "Folder";
        public string Template => "Blocks/Forms/folder";
        public IItem CreateNew(IDirectoryItem directory, IRequest request)
        {
            if(string.IsNullOrEmpty(request.PostArgs["folder-name"]))
                throw new InvalidOperationException("folder-name is not specified");

            return DirectoryItem.Create(directory.Owner, Path.Combine(directory.RelativePath, request.PostArgs["folder-name"]));
        }
    }
}
