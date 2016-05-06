using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Framework;

namespace Memorandum.Web.Creators
{
    class UploaderCreator : ICreator
    {
        public string Id => "upload";
        public string Name => "Upload";
        public string Template => "Blocks/Forms/upload";

        public IItem CreateNew(IDirectoryItem directory, IRequest request)
        {
            // Save passed files
            foreach (var file in request.Files)
            {
                var fileItem = new FileItem(directory.Owner, Path.Combine(directory.RelativePath, file.FileName));
                // if(fileItem.Exists) // TODO: overwrite ?
                using (var writer = fileItem.GetStream(FileMode.OpenOrCreate))
                {
                    file.Data.CopyTo(writer);
                }
            }

            return directory;
        }
    }
}
