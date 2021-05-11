using System.Collections.Generic;
using System.IO;

namespace Polo.Comparers
{
    public class FileNameComparer : IComparer<string>
    {
        // TODO LA - Cover with UTs
        public int Compare(string x, string y)
        {
            var fileNameX = Path.GetFileName(x);
            var fileNameY = Path.GetFileName(y);

            return string.Compare(fileNameX, fileNameY);
        }
    }
}
