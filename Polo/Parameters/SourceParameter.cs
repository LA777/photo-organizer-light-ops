using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class SourceParameter : IParameter<string>
    {
        public string Name => "source";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { @"""c:\images""" };

        public string Description => "Source folder full path.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.DefaultSourceFolderPath)}'.");
            }

            if (!Directory.Exists(outputValue))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{outputValue}' does not exist.");
            }

            return outputValue;
        }
    }
}