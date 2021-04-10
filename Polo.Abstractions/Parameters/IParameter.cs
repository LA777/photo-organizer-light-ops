using System.Collections.Generic;

namespace Polo.Abstractions.Parameters
{
    public interface IParameter<T>
    {
        public T Initialize(IReadOnlyDictionary<string, string> inputParameters, T defaultValue);
    }
}
