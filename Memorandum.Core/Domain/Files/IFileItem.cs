using System.IO;

namespace Memorandum.Core.Domain.Files
{
    public interface IFileItem : IItem
    {
        string NameWithoutExtension { get; }
        string Extension { get; }
        string Mime { get; }
        long Size { get; }

        Stream GetStream(FileMode mode = FileMode.Open);
    }
}