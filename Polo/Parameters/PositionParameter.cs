using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class PositionParameter : IParameter<string>
    {
        public string Name => "position";

        public IReadOnlyCollection<string> PossibleValues => new List<string>
        {
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

        public string Description => "Describes position.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
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