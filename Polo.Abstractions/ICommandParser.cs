using Polo.Abstractions.Commands;

namespace Polo.Abstractions;

public interface ICommandParser
{
    public Task ParseAsync(string[] arguments, IEnumerable<ICommand> commands);
}
