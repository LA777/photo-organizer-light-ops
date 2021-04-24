using Polo.Abstractions.Commands;
using Serilog;
using System;
using System.Collections.Generic;

namespace Polo.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly ILogger _logger;

        public string Name => "help";

        public string ShortName => "h";

        public string Description => "Shows list of available commands.";

        public HelpCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            foreach (var command in commands)
            {
                var line = $"{CommandParser.ShortCommandPrefix}{command.ShortName},\t{CommandParser.CommandPrefix}{command.Name}\t\t\t{command.Description}";
                _logger.Verbose(line);
                Console.WriteLine(line);
            }
        }
    }
}
