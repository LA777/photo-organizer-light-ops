using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Exceptions;
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
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (arguments.Length > 1)
            {
                parameters = ParseParameters(arguments.Skip(1).ToArray());
            }

            var matchedCommand = commands.FirstOrDefault(x => $"{CommandPrefix}{x.Name}" == commandArgument);
            if (matchedCommand != null)
            {
                _logger.Verbose($"Argument '{commandArgument}' matched to command '{matchedCommand.Name}'");
                matchedCommand.Action(parameters, commands);
            }
            else
            {
                var matchedShortCommand = commands.FirstOrDefault(x => $"{ShortCommandPrefix}{x.ShortName}" == commandArgument);

                if (matchedShortCommand != null)
                {
                    _logger.Verbose($"Argument '{commandArgument}' matched to command '{matchedShortCommand.Name}'");
                    matchedShortCommand.Action(parameters, commands);
                }
                else
                {
                    throw new ParseException("ERROR: Unknown command. Please enter --help to see available commands list.");
                }
            }
        }

        private Dictionary<string, string> ParseParameters(string[] arguments)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var argument in arguments)
            {
                if (!argument.StartsWith(ShortCommandPrefix))
                {
                    throw new ParseException("ERROR: Unknown parameter. Please enter --help to see available parameters list.");
                }
                else
                {
                    var argumetWithoutPrefix = argument.TrimStart(ShortCommandPrefix.ToCharArray().First());
                    var split = argumetWithoutPrefix.Split(ParameterDelimiter);
                    string value;

                    if (split.Count() < 2)
                    {
                        throw new ParseException("ERROR: Parameter delimiter missed. Please enter --help to see correct parameter syntax.");
                    }

                    if (split.Count() > 2)
                    {
                        value = string.Join(ParameterDelimiter, split.Skip(1));
                    }
                    else
                    {
                        value = split[1];
                    }

                    _logger.Verbose($"Parameter name: '{split[0]}' value: '{value}'");
                    dictionary.Add(split[0], value);
                }
            }

            return dictionary;
        }
    }
}
