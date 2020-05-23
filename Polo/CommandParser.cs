using System;
using System.Linq;

namespace Polo
{
    public class CommandParser
    {
        private const string ShortCommandPrefix = "-";
        private const string CommandPrefix = "--";

        public static void Parse(string[] arguments)
        {
            var commands = SupportedCommands.GetCommandsList();

            foreach (var argument in arguments)
            {
                var matchedCommand = commands.FirstOrDefault(x => $"{CommandPrefix}{x.Name}" == argument);
                if (matchedCommand != null)
                {
                    matchedCommand.Action();
                }
                else
                {
                    var matchedShortCommand = commands.FirstOrDefault(x => $"{ShortCommandPrefix}{x.ShortName}" == argument);
                    if (matchedShortCommand != null)
                    {
                        matchedShortCommand.Action();
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Unknown command. Please enter --help to see available commands list.");
                    }
                }
            }
        }
    }
}
