using Polo.Abstractions.Commands;

namespace Polo.Abstractions;

public interface ISupportedCommands
{
    public IEnumerable<ICommand> GetCommandsList();
}
