using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class OutputFolderNameParameter : IParameter<string> // TODO LA - Refactor to OutputSubFolderNameParameter
    {
        public string Name => "output-folder-name";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { "output", "processed" };

        public string Description => "Name of the folder, where all processed files will be placed.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.OutputSubfolderName)}'.");
            }

            return outputValue;
        }
    }
}