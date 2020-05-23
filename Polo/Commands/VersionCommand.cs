using System;

namespace Polo.Commands
{
    public class VersionCommand : ICommand
    {
        private const string Version = "0.0.1";

        public string Name => "version";

        public string ShortName => "v";

        public string Description => "Shows application version.";

        public void Action()
        {
            Console.WriteLine(Version);
        }
    }
}
