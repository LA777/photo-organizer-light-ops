using System;

namespace Polo.Abstractions.Exceptions
{
    public class ParameterAbsentException : Exception
    {
        public ParameterAbsentException() { }

        public ParameterAbsentException(string message) : base(message) { }

        public ParameterAbsentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
