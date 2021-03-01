using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using System.Collections.Generic;
using System.IO;

namespace Polo.Parameters
{
    public class WatermarkPathParameter : IParameter<string>, IParameterInfo
    {
        public static string Name => "watermark-path";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() { @"c:\sign\my_watermark.png" };

        public static string Description => "Watermark image full path.";

        public string Initialize(IReadOnlyDictionary<string, string> incomeParameters, string defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = incomeParameters.TryGetValue(Name, out var parameterValue)
                ? parameterValue : defaultValue;

            if (string.IsNullOrEmpty(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter."); // TODO LA - Refactor - Add setting parameter
            }

            if (!File.Exists(outputValue))
            {
                throw new FileNotFoundException("ERROR: Watermark file not found.", outputValue);
            }

            return outputValue;
        }
    }
}
