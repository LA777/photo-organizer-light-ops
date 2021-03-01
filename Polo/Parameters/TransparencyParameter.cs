using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class TransparencyParameter : IParameter<int>, IParameterInfo
    {
        private static readonly int _min = 1;// TODO LA - Combine min and max with Settings
        private static readonly int _max = 100;

        public static string Name => "watermark-path";

        public static IReadOnlyCollection<int> PossibleValues => new List<int>() { _min, 42, _max };

        public static string Description => "Watermark image full path.";

        public int Initialize(IReadOnlyDictionary<string, string> incomeParameters, int defaultValue)
        {
            // TODO LA - Cover with UTs

            int outputValue;
            if (incomeParameters.TryGetValue(Name, out var parameterValue))
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
            else
            {
                outputValue = defaultValue;
            }

            if (outputValue < _min || outputValue > _max)
            {
                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be in the range from {_min} to {_max}."); // TODO LA - Refactor
            }

            return outputValue;
        }
    }
}
