using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Globalization;

namespace Polo.Parameters
{
    public class MegaPixelsLimitParameter : IParameter<float>
    {
        public static float Min => 0.001f;

        public string Name => "mega-pixels-limit";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { Min.ToString(CultureInfo.InvariantCulture), "16" };

        public string Description => "Mega pixels limit for image resolution.";

        public float Initialize(IReadOnlyDictionary<string, string> inputParameters, float defaultValue, IEnumerable<ICommand> commands = null!)
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