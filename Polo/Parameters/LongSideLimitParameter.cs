using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters;

public class LongSideLimitParameter : IParameter<uint>
{
    public static uint Min => 1;

    public string Name => "long-side-limit";

    public IReadOnlyCollection<string> PossibleValues => [Min.ToString(), "1200"];

    public string Description => "Long side limit for image resize.";

    public uint Initialize(IReadOnlyDictionary<string, string> inputParameters, uint defaultValue, IEnumerable<ICommand> commands = null!)
    {
        var outputValue = defaultValue;

        if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
        {
            if (uint.TryParse(parameterValue, out var number))
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