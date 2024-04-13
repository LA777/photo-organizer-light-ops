using Polo.Abstractions.Commands;
using Polo.Abstractions.Enums;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class OutputFileTypeParameter : IParameter<OutputFileType>
    {
        public string Name => "output-file-type";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { OutputFileType.SQLITE.ToString(), OutputFileType.JSON.ToString() };

        public string Description => "Output file type.";

        public OutputFileType Initialize(IReadOnlyDictionary<string, string> inputParameters, OutputFileType defaultValue, IEnumerable<ICommand> commands = null!)
        {
            var outputValue = defaultValue;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                if (OutputFileType.SQLITE.ToString().ToUpper() == parameterValue.ToUpper())
                {
                    return OutputFileType.SQLITE;
                }
                else if (OutputFileType.JSON.ToString().ToUpper() == parameterValue.ToUpper())
                {
                    return OutputFileType.JSON;
                }
                else if (OutputFileType.CSV.ToString().ToUpper() == parameterValue.ToUpper())
                {
                    return OutputFileType.CSV;
                }
                else if (OutputFileType.TXT.ToString().ToUpper() == parameterValue.ToUpper())
                {
                    return OutputFileType.TXT;
                }

                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' has ambiguous value.");
            }

            return outputValue;
        }
    }
}