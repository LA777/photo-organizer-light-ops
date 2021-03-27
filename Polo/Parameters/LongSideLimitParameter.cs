using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class LongSideLimitParameter : IParameter<int>, IParameterInfo
    {
        private static readonly int _min = 1;

        public static string Name => "long-side-limit";

        public static IReadOnlyCollection<int> PossibleValues => new List<int>() { _min, 1200 };

        public static string Description => "Long side limit for image resize.";

        public int Initialize(IReadOnlyDictionary<string, string> incomeParameters, int defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = defaultValue;
            var parametersEmpty = incomeParameters.IsNullOrEmpty();

            if (!parametersEmpty && incomeParameters.TryGetValue(Name, out var parameterValue))
            {
                if (int.TryParse(parameterValue, out var number))
                {
                    outputValue = number;
                }
                else
                {
                    throw new ParseException($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' is not a number.");
                }
            }

            if (outputValue < _min)
            {
                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be in the range from {_min} to image long side width."); // TODO LA - Refactor
            }

            return outputValue;
        }
    }
}
