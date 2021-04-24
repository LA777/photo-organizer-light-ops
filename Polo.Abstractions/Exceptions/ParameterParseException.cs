using System;

namespace Polo.Abstractions.Exceptions
{
    public class ParameterParseException : Exception
    {
        public ParameterParseException()
        {
        }

        public ParameterParseException(string message) : base(message)
        {
        }

        public ParameterParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
