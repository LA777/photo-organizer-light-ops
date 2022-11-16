using Polo.Abstractions.Commands;

namespace Polo.Abstractions.Parameters
{
    public interface IParameter<T> : IParameterInfo
    {
        public T Initialize(IReadOnlyDictionary<string, string> inputParameters, T defaultValue, IEnumerable<ICommand> commands = null!);
    }
}