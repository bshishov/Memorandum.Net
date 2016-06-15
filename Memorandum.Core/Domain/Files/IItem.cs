using System;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Files
{
    public interface IItem
    {
        User Owner { get; }
        string RelativePath { get; }
        string FileSystemPath { get; }
        bool IsLink { get; }
        string Name { get; }
        DateTime Created { get; }
        DateTime Modified { get; }
        bool IsDirectory { get; } // ???????????????
        bool Exists { get; }

        void Rename(string newName);
        void MoveTo(IDirectoryItem target);
        void CopyTo(IDirectoryItem target);
        void Delete();
        IDirectoryItem GetParent();

        string GetHash();
        bool Equals(IItem item);
    }
}