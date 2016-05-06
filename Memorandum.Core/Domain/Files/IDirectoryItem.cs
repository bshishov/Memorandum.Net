using System;
using System.Collections.Generic;

namespace Memorandum.Core.Domain.Files
{
    public interface IDirectoryItem : IItem
    {
        bool IsRoot { get; }
        IFileItem Index { get; }

        void PerformOnchild(Action<IItem> action, bool recursive = false);

        IEnumerable<IItem> GetChild();
        IEnumerable<IDirectoryItem> GetChildDirectories();
        IEnumerable<IFileItem> GetChildFiles();
    }
}