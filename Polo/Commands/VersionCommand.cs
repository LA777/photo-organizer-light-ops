using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters.Handler;
using Serilog;
using System.Reflection;

namespace Polo.Commands
{
    public class VersionCommand : ICommand
    {
        public const string NameLong = "version";
        public const string NameShort = "v";
        private readonly ILogger _logger;

        public VersionCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Shows application version.";

        public IParameterHandler ParameterHandler { get; }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _logger.Verbose(version?.ToString());
            Console.WriteLine(version?.ToString());
        }
    }
}