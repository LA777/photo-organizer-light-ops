using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Polo.Comparers
{
    public class FileNameComparer : IComparer<string>, IEqualityComparer<string>
    {
        // TODO LA - Cover with UTs
        public int Compare(string x, string y)
        {
            var fileNameX = Path.GetFileName(x);
            var fileNameY = Path.GetFileName(y);

            return string.Compare(fileNameX, fileNameY, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(string x, string y)
        {
            var fileNameWithoutExtensionX = Path.GetFileName(x);
            var fileNameWithoutExtensionY = Path.GetFileName(y);

            return string.Equals(fileNameWithoutExtensionX, fileNameWithoutExtensionY, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] string fileFullPath)
        {
            var fileName = Path.GetFileName(fileFullPath).ToUpper();

            return fileName.GetHashCode();
        }
    }
}
