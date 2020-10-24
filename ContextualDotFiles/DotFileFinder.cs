using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContextualDotFiles
{
    internal static class DotFileFinder
    {
        internal static IEnumerable<FileInfo> FindAll(DirectoryInfo rootDir, Regex regex)
        {
            return _FindAll(rootDir);

            IEnumerable<FileInfo> _FindAll(DirectoryInfo dir)
            {
                if (dir.Exists)
                {
                    var found = dir.EnumerateFiles()
                        .Where(f => regex.IsMatch(f.Name));

                    return dir.Parent != null
                        ? _FindAll(dir.Parent).Concat(found)
                        : found;
                }

                return Enumerable.Empty<FileInfo>();
            }
        }
    }
}