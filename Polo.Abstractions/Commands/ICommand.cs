using System.Collections.Generic;

namespace Polo.Abstractions.Commands
{
    public interface ICommand
    {
        public string Name { get; }
        public string ShortName { get; }
        public string Description { get; }
        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null);
    }
}
