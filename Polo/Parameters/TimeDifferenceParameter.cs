using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class TimeDifferenceParameter : IParameter<double>, IParameterInfo
    {
        public static double Min => -24f;// TODO LA - Combine min and max with Options validation
        public static double Max => 24f;

        public static string Name => "time-difference";

        public static IReadOnlyCollection<double> PossibleValues => new List<double>() { Min, 4.5f, Max };

        public static string Description => "Time difference in hours.";

        public double Initialize(IReadOnlyDictionary<string, string> inputParameters, double defaultValue = 0)
        {
            // TODO LA - Cover with UTs
            var outputValue = defaultValue;
            var parametersEmpty = inputParameters.IsNullOrEmpty();

            if (!parametersEmpty && inputParameters.TryGetValue(Name, out var parameterValue))
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

            if (outputValue < Min || outputValue > Max)
            {
                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be in the range from '{Min}' to '{Max}'.");
            }

            return outputValue;
        }
    }
}
