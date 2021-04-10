using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class LongSideLimitParameter : IParameter<int>, IParameterInfo
    {
        public static int Min => 1;

        public static string Name => "long-side-limit";

        public static IReadOnlyCollection<int> PossibleValues => new List<int>() { Min, 1200 };

        public static string Description => "Long side limit for image resize.";

        public int Initialize(IReadOnlyDictionary<string, string> inputParameters, int defaultValue)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                if (int.TryParse(parameterValue, out var number))
                {
                    outputValue = number;
                }
                else
                {
                    throw new ParameterParseException($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' is not a number.");
                }
            }

            if (outputValue < Min)
            {
                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be higher than {Min - 1}.");
            }

            return outputValue;
        }
    }
}
