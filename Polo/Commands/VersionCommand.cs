using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters.Handler;
using Polo.Abstractions.Wrappers;
using Serilog;
using System.Reflection;

namespace Polo.Commands
{
    public class VersionCommand : ICommand
    {
        public const string NameLong = "version";
        public const string NameShort = "v";
        private readonly IConsoleWrapper _consoleWrapper;
        private readonly ILogger _logger;

        public VersionCommand(IConsoleWrapper consoleWrapper, ILogger logger)
        {
            _consoleWrapper = consoleWrapper ?? throw new ArgumentNullException(nameof(consoleWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Shows application version.";

        public IParameterHandler ParameterHandler { get; } = null!;

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            var versionText = version.ToString();
            _logger.Verbose(versionText);
            _consoleWrapper.WriteLine(versionText);
        }
    }
}