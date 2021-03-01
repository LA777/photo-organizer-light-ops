using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class PositionParameter : IParameter<string>, IParameterInfo
    {
        public static string Name => "position";

        public static IReadOnlyCollection<string> PossibleValues => new List<string>() {
            "right-bottom",
            "right-center",
            "right-top",
            "left-top",
            "left-center",
            "left-bottom",
            "center-top",
            "center-center",
            "center-bottom"
        };

        public static string Description => "Describes position.";

        public string Initialize(IReadOnlyDictionary<string, string> incomeParameters, string defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = defaultValue;
            var parametersEmpty = incomeParameters.IsNullOrEmpty();

            if (parametersEmpty && string.IsNullOrWhiteSpace(defaultValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPosition)}'.");  // TODO LA - Refactor
            }

            if (!parametersEmpty && incomeParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPosition)}'."); // TODO LA - Refactor
            }

            return outputValue;
        }
    }
}
