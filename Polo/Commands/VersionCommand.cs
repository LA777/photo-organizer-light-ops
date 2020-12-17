using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using System;
using System.Collections.Generic;

namespace Polo.Commands
{
    public class VersionCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        private const string Version = "0.0.2";

        public string Name => "version";

        public string ShortName => "v";

        public string Description => "Shows application version.";

        public VersionCommand(IConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            _consoleService.WriteLine(Version);
        }
    }
}
