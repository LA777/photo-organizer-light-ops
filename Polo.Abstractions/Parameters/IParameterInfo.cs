using System.Collections.Generic;

namespace Polo.Abstractions.Parameters
{
    public interface IParameterInfo
    {
        // TODO LA - Remove
        public static string Name { get; }

        public static IReadOnlyCollection<string> PossibleValues { get; }

        public static string Description { get; }
    }
}
