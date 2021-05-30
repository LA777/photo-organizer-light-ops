using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class OutputFolderNameParameter : IParameter<string>, IParameterInfo // TODO LA - Refactor to OutputFolderPathParameter
    {
        public static string Name => "output-folder-name";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() { "output", "processed" };

        public static string Description => "Name of the folder, where all processed files will be placed.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkOutputFolderName)}'.");
            }

            return outputValue;
        }
    }
}
