using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Globalization;

namespace Polo.Parameters
{
    public class TimeDifferenceParameter : IParameter<double>
    {
        public static double Min => -24f; // TODO LA - Combine min and max with Options validation
        public static double Max => 24f;

        public string Name => "time-difference";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { Min.ToString(CultureInfo.InvariantCulture), "4.5f", Max.ToString(CultureInfo.InvariantCulture) };

        public string Description => "Time difference in hours.";

        public double Initialize(IReadOnlyDictionary<string, string> inputParameters, double defaultValue = 0, IEnumerable<ICommand> commands = null!)
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