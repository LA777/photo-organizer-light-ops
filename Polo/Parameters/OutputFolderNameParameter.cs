using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class OutputFolderNameParameter : IParameter<string>, IParameterInfo
    {
        public static string Name => "output-folder-name";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() { "output" };

        public static string Description => "Output folder name.";

        public string Initialize(IReadOnlyDictionary<string, string> incomeParameters, string defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = incomeParameters.TryGetValue(Name, out var parameterValue)
                ? parameterValue : defaultValue;

            if (string.IsNullOrEmpty(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter."); // TODO LA - Refactor - Add setting parameter
            }

            return outputValue;
        }
    }
}
