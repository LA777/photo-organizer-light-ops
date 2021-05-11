using Polo.Comparers;
using System.Collections.Generic;

namespace Polo.Extensions
{
    public static class ListExtension
    {
        private static FileNameComparer _fileNameComparer = new FileNameComparer();

        public static void SortByFileName(this List<string> collection)
        {
            collection.Sort(_fileNameComparer);
        }
    }
}
