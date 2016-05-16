using Memorandum.Core.Domain.Files;
using Shine;

namespace Memorandum.Web.Creators
{
    interface ICreator
    {
        string Id { get; }
        string Name { get; }
        string Template { get; }
        IItem CreateNew(IDirectoryItem directory, IRequest request);
    }
}