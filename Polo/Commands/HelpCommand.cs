using System;

namespace Polo.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";

        public string ShortName => "h";

        public string Description => "Shows list of available commands.";

        public void Action()
        {
            var commands = SupportedCommands.GetCommandsList();

            foreach (var command in commands)
            {
                Console.WriteLine($"-{command.ShortName}, --{command.Name}\t\t{command.Description}");
            }
        }
    }
}
