using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class ExtensionParameter : IParameter<string>
    {
        public string Name => "extension";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { "jpg", "mp4" };

        public string Description => "File extension.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null!)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter.");
            }

            return outputValue;
        }
    }
}