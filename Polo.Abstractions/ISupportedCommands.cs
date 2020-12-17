using Polo.Abstractions.Commands;
using System.Collections.Generic;

namespace Polo.Abstractions
{
    public interface ISupportedCommands
    {
        public IEnumerable<ICommand> GetCommandsList();
    }
}
