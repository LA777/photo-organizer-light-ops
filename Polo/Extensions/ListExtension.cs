using Polo.Comparers;

namespace Polo.Extensions
{
    public static class ListExtension
    {
        private static readonly FileNameComparer FileNameComparer = new();

        public static void SortByFileName(this List<string> collection)
        {
            collection.Sort(FileNameComparer);
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}