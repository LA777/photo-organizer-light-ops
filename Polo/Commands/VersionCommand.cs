using Polo.Abstractions.Commands;
using Serilog;
using System;
using System.Collections.Generic;

namespace Polo.Commands
{
    public class VersionCommand : ICommand
    {
        private readonly ILogger _logger;

        public string Name => "version";

        public string ShortName => "v";

        public string Description => "Shows application version.";

        public VersionCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            _logger.Information(Program.Version);
        }
    }
}
