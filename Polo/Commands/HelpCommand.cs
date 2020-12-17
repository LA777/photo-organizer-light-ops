using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using System;
using System.Collections.Generic;

namespace Polo.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IConsoleService _consoleService;

        public string Name => "help";

        public string ShortName => "h";

        public string Description => "Shows list of available commands.";

        public HelpCommand(IConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            foreach (var command in commands)
            {
                _consoleService.WriteLine($"-{command.ShortName}, --{command.Name}\t\t{command.Description}");
            }
        }
    }
}
