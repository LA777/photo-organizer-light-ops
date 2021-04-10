using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Polo.Parameters
{
    public class DestinationParameter : IParameter<string>, IParameterInfo
    {
        public static string Name => "destination";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() { @"c:\photo" };

        public static string Description => "Destination folder full path.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue)
        {
            // TODO LA - Cover with UTs
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter.");
            }

            if (!Directory.Exists(outputValue))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{outputValue}' does not exist.");
            }

            return outputValue;
        }
    }
}
