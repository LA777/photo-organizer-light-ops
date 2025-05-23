using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters;

public class FsivThumbnailSizeParameter : IParameter<uint>
{
    private readonly List<uint> _possibleValues = [Min, 80, 120, 160, 200, Max];
    public static uint Min => 60;
    public static uint Max => 260;

    public string Name => "fsiv-thumbnail-size";

    public IReadOnlyCollection<string> PossibleValues => [.. _possibleValues.Select(x => x.ToString())];

    public string Description => "FS Image Viewer thumbnail size parameter.";

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

        if (!_possibleValues.Contains(outputValue))
        {
            throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be one of any possible values. See HELP.");
        }

        return outputValue;
    }
}