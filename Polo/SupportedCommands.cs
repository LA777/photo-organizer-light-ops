using Polo.Commands;
using System.Collections.Generic;

namespace Polo
{
    public class SupportedCommands
    {
        private static readonly IEnumerable<ICommand> Commands = new List<ICommand>
        {
            new VersionCommand(),
            new HelpCommand(),
            new RawCommand(),
            new RemoveOrphanageRawCommand(),
            new MoveRawToJpegFolderCommand()
        };

        public static IEnumerable<ICommand> GetCommandsList()
        {
            return Commands;
        }
    }
}
