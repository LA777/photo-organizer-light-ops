using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polo
{
    public class CommandParser : ICommandParser
    {
        private readonly ILogger _logger;
        public const string ShortCommandPrefix = "-";
        public const string CommandPrefix = "--";
        public const string ParameterDelimiter = ":";

        public CommandParser(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Parse(string[] arguments, IEnumerable<ICommand> commands)
        {
            if (arguments == null || !arguments.Any())
            {
                throw new ParseException("ERROR: No command provided. Please enter --help to see available commands list.");
            }

            var commandArgument = arguments.First();
            var parameters = new Dictionary<string, string>();
            var commandsList = commands.ToList();

            if (arguments.Length > 1)
            {
                parameters = ParseParameters(arguments.Skip(1).ToArray());
            }

            var matchedCommand = commandsList.FirstOrDefault(x => $"{CommandPrefix}{x.Name}" == commandArgument);
            if (matchedCommand != null)
            {
                _logger.Verbose($"Argument '{commandArgument}' matched to command '{matchedCommand.Name}'");
                matchedCommand.Action(parameters, commandsList);
            }
            else
            {
                var matchedShortCommand = commandsList.FirstOrDefault(x => $"{ShortCommandPrefix}{x.ShortName}" == commandArgument);

                if (matchedShortCommand != null)
                {
                    _logger.Verbose($"Argument '{commandArgument}' matched to command '{matchedShortCommand.Name}'");
                    matchedShortCommand.Action(parameters, commandsList);
                }
                else
                {
                    throw new ParseException("ERROR: Unknown command. Please enter --help to see available commands list."); // TODO LA - Refactor
                }
            }
        }

        private Dictionary<string, string> ParseParameters(IEnumerable<string> arguments)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var argument in arguments)
            {
                if (!argument.StartsWith(ShortCommandPrefix))
                {
                    throw new ParseException("ERROR: Unknown parameter. Please enter --help to see available parameters list."); // TODO LA - Refactor
                }

                var argumentWithoutPrefix = argument.TrimStart(ShortCommandPrefix.ToCharArray().First());
                var split = argumentWithoutPrefix.Split(ParameterDelimiter);

                if (split.Length < 2)
                {
                    throw new ParseException("ERROR: Parameter delimiter missed. Please enter --help to see correct parameter syntax."); // TODO LA - Refactor
                }

                var value = split.Length > 2 ? string.Join(ParameterDelimiter, split.Skip(1)) : split[1];

                _logger.Verbose($"Parameter name: '{split[0]}' value: '{value}'");
                dictionary.Add(split[0], value);
            }

            return dictionary;
        }
    }
}
