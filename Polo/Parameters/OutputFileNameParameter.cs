using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters;

public class OutputFileNameParameter : IParameter<string>
{
    public string Name => "output-file-name";

    public IReadOnlyCollection<string> PossibleValues => ["output.txt"];

    public string Description => "Output file name.";

    public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null)
    {
        var outputValue = defaultValue;

        if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
        {
            outputValue = parameterValue;
        }

        if (string.IsNullOrWhiteSpace(outputValue))
        {
            throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.OutputFileName)}'.");
        }

        return outputValue;
    }
}