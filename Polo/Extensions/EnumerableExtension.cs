using System.Collections.Generic;
using System.Linq;

namespace Polo.Extensions
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            // TODO LA - Cover with UTs
            return collection == null || !collection.Any();
        }
    }
}
