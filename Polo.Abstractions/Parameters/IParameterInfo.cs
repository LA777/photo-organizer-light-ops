using Polo.Abstractions.Commands;

namespace Polo.Abstractions.Parameters
{
    public interface IParameterInfo
    {
        public string Name { get; }

        public IReadOnlyCollection<string> PossibleValues { get; }

        public string Description { get; }
    }
}