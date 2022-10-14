using Polo.Abstractions.Parameters.Handler;

namespace Polo.Abstractions.Commands
{
    public interface ICommand
    {
        public string Name { get; }

        public string ShortName { get; }

        public string Description { get; }

        public IParameterHandler ParameterHandler { get; }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null);
    }
}