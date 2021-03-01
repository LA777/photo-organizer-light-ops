using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using System.Collections.Generic;
using System.IO;

namespace Polo.Parameters
{
    public class SourceParameter : IParameter<string>, IParameterInfo
    {
        public static string Name => "source";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() { @"c:\images" };

        public static string Description => "Source folder full path.";

        public string Initialize(IReadOnlyDictionary<string, string> incomeParameters, string defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = incomeParameters.TryGetValue(Name, out var parameterValue)
                ? parameterValue : defaultValue;

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
