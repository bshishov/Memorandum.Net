using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Memorandum.Core.Utilities
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> EnumerateFilesSafe(this DirectoryInfo dir, string filter = "*", SearchOption opt = SearchOption.TopDirectoryOnly)
        {
            var retval = Enumerable.Empty<FileInfo>();

            try { retval = dir.EnumerateFiles(filter, opt); }
            catch { Debug.WriteLine("{0} Inaccessable.", dir.FullName); }

            if (opt == SearchOption.AllDirectories)
                retval = retval.Concat(dir.EnumerateDirectoriesSafe(opt: opt).SelectMany(x => x.EnumerateFilesSafe(filter, opt)));

            return retval;
        }

        public static IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(this DirectoryInfo dir, string filter = "*", SearchOption opt = SearchOption.TopDirectoryOnly)
        {
            var retval = Enumerable.Empty<DirectoryInfo>();

            try { retval = dir.EnumerateDirectories(filter, opt); }
            catch { Debug.WriteLine("{0} Inaccessable.", dir.FullName); }

            if (opt == SearchOption.AllDirectories)
                retval = retval.Concat(retval.SelectMany(x => x.EnumerateDirectoriesSafe(filter, opt)));

            return retval;
        }
    }
}
