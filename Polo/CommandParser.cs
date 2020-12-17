using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polo
{
    public class CommandParser : ICommandParser
    {
        private readonly IConsoleService _consoleService;
        private const string ShortCommandPrefix = "-";
        private const string CommandPrefix = "--";

        public CommandParser(IConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Parse(string[] arguments, IEnumerable<ICommand> commands)
        {
            var argument = arguments.First();

            var matchedCommand = commands.FirstOrDefault(x => $"{CommandPrefix}{x.Name}" == argument);
            if (matchedCommand != null)
            {
                matchedCommand.Action(arguments, commands);
            }
            else
            {
                var matchedShortCommand = commands.FirstOrDefault(x => $"{ShortCommandPrefix}{x.ShortName}" == argument);
                if (matchedShortCommand != null)
                {
                    matchedShortCommand.Action(arguments, commands);
                }
                else
                {
                    _consoleService.WriteLine("ERROR: Unknown command. Please enter --help to see available commands list.");
                }
            }
        }
    }
}
