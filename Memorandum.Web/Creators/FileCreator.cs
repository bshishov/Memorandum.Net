using System.IO;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Framework;

namespace Memorandum.Web.Creators
{
    abstract class FileCreator: ICreator
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Template { get; }

        public IItem CreateNew(IDirectoryItem directory, IRequest request)
        {
            var fileName = request.PostArgs["name"]; // TODO: validate!
            if (!Path.HasExtension(fileName))
                fileName += DefaultExtension;
            return FileItem.Create(directory.Owner, Path.Combine(directory.RelativePath, fileName));
        }

        public abstract string DefaultExtension { get; }
    }
}
