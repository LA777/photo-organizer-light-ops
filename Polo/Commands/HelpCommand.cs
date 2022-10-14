using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System.Reflection;
using System.Text;

namespace Polo.Commands
{
    public class HelpCommand : ICommand
    {
        public const string NameLong = "help";
        public const string NameShort = "h";
        private readonly ILogger _logger;


        public HelpCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Shows list of available commands.";

        public IParameterHandler ParameterHandler { get; } = new ParameterHandler
        {
            CommandParameter = new CommandParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            var stringBuilder = new StringBuilder();

            if (parameters.IsNullOrEmpty())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("POLO - photo organizer light operations.");
                stringBuilder.AppendLine("Utility that helps arrange and manage images.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Commands:");

                foreach (var command in commands.OrderBy(x => x.Name))
                {
                    stringBuilder.AppendLine($"\tCommand Name:\t{CommandParser.CommandPrefix}{command.Name}");
                    stringBuilder.AppendLine($"\tShort Name:\t{CommandParser.ShortCommandPrefix}{command.ShortName}");
                    stringBuilder.AppendLine($"\tDescription:\t{command.Description}");
                    stringBuilder.AppendLine();
                }

                DisplayAndLogText(stringBuilder.ToString());
            }
            else
            {
                if (ParameterHandler == null)
                {
                    throw new ArgumentException();
                }

                if (commands == null)
                {
                    throw new ArgumentException();
                }

                var parameterCommand = ParameterHandler.CommandParameter.Initialize(parameters, null, commands);

                if (parameterCommand == null)
                {
                    throw new ArgumentException();
                }

                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Command Name:\t{CommandParser.CommandPrefix}{parameterCommand.Name}");
                stringBuilder.AppendLine($"Short Name:\t{CommandParser.ShortCommandPrefix}{parameterCommand.ShortName}");
                stringBuilder.AppendLine($"Description:\t{parameterCommand.Description}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Example:");

                var applicationName = Assembly.GetExecutingAssembly().GetName().Name?.ToLower() ?? "polo";
                stringBuilder.Append($"{applicationName} {CommandParser.CommandPrefix}{parameterCommand.Name} ");

                var commandParameters = parameterCommand.ParameterHandler.GetParameters()
                    .OrderBy(x => x.Name).ToList();

                foreach (var parameter in commandParameters)
                {
                    stringBuilder.Append($"{CommandParser.ShortCommandPrefix}{parameter.Name}{CommandParser.ParameterDelimiter}{parameter.PossibleValues.Last()} ");
                }

                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Parameters:");


                foreach (var parameter in commandParameters)
                {
                    stringBuilder.AppendLine($"\tParameter Name:\t\t{CommandParser.ShortCommandPrefix}{parameter.Name}");
                    stringBuilder.AppendLine($"\tDescription:\t\t{parameter.Description}");
                    var possibleValues = string.Join("; ", parameter.PossibleValues);
                    stringBuilder.AppendLine($"\tPossible values:\t{possibleValues}");
                    stringBuilder.AppendLine();
                }

                DisplayAndLogText(stringBuilder.ToString());
            }
        }

        private void DisplayAndLogText(string text)
        {
            _logger.Verbose(text);
            Console.WriteLine(text);
        }
    }
}