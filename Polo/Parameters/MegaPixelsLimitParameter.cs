using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class MegaPixelsLimitParameter : IParameter<float>, IParameterInfo
    {
        public static float Min => 0.001f;

        public static string Name => "mega-pixels-limit";

        public static IReadOnlyCollection<float> PossibleValues => new List<float>() { Min, 16 };

        public static string Description => "Mega pixels limit for image resolution.";

        public float Initialize(IReadOnlyDictionary<string, string> inputParameters, float defaultValue)
        {
            // TODO LA - Cover with UTs
            // TODO LA - Refactor - Use abstract class for int parameters
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                if (float.TryParse(parameterValue, out var number))
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
