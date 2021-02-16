using Polo.Abstractions.Commands;
using System.Collections.Generic;

namespace Polo.Abstractions
{
    public interface ICommandParser
    {
        public void Parse(string[] arguments, IEnumerable<ICommand> commands);
    }
}
