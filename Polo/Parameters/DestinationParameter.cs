using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class DestinationParameter : IParameter<string>
    {
        public string Name => "destination";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { @"""c:\photo""" };

        public string Description => "Destination folder full path.";

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

            if (!Directory.Exists(outputValue))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{outputValue}' does not exist.");
            }

            return outputValue;
        }
    }
}