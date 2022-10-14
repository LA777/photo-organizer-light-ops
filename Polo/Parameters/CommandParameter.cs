using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Commands;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class CommandParameter : IParameter<ICommand>
    {
        public string Name => "command";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { ResizeCommand.NameLong, ResizeCommand.NameShort };

        public string Description => "Name of a command.";

        public ICommand Initialize(IReadOnlyDictionary<string, string> inputParameters, ICommand defaultValue, IEnumerable<ICommand> commands)
        {
            var commandName = string.Empty;

            if (!inputParameters.IsNullOrEmpty() && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                commandName = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter.");
            }

            var command = GetCommandByName(commandName, commands);

            if (command == null)
            {
                throw new ArgumentException("ERROR: No such command name.");
            }

            return command;
        }

        private static ICommand GetCommandByName(string commandName, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                throw new ArgumentException("ERROR: Argument is NULL or EMPTY", nameof(commands));
            }

            var commandsList = commands.ToList();
            var matchedCommand = commandsList.FirstOrDefault(x => x.Name == commandName);
            if (matchedCommand != null)
            {
                return matchedCommand;
            }

            var matchedShortCommand = commandsList.First(x => x.ShortName == commandName);

            return matchedShortCommand;
        }
    }
}