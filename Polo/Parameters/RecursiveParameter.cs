using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System.Globalization;

namespace Polo.Parameters
{
    public class RecursiveParameter : IParameter<bool>
    {
        public string Name => "recursive";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { true.ToString(CultureInfo.InvariantCulture), false.ToString(CultureInfo.InvariantCulture) };

        public string Description => "Run command recursively for all subfolders.";

        public bool Initialize(IReadOnlyDictionary<string, string> inputParameters, bool defaultValue, IEnumerable<ICommand> commands = null!)
        {
            // TODO LA - Cover with UTs
            var parametersEmpty = inputParameters.IsNullOrEmpty();

            if (parametersEmpty || !inputParameters.TryGetValue(Name, out var parameterValue))
            {
                return defaultValue;
            }

            if (!bool.TryParse(parameterValue, out var parsedValue))
            {
                throw new ParameterParseException($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' is not valid.");
            }

            return parsedValue;
        }
    }
}