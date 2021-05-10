using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Polo.Comparers
{
    public class FileNameWithoutExtensionComparer : IEqualityComparer<string>
    {
        // TODO LA - Cover with UTs
        public bool Equals(string x, string y)
        {
            var fileNameWithoutExtensionX = Path.GetFileNameWithoutExtension(x);
            var fileNameWithoutExtensionY = Path.GetFileNameWithoutExtension(y);

            return string.Equals(fileNameWithoutExtensionX, fileNameWithoutExtensionY, StringComparison.OrdinalIgnoreCase);
        }

        // TODO LA - Cover with UTs
        public int GetHashCode([DisallowNull] string fileFullPath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileFullPath);

            return fileNameWithoutExtension.GetHashCode();
        }
    }
}
