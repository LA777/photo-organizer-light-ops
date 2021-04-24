using System;

namespace Polo.Abstractions.Exceptions
{
    public class ImageFormatException : Exception
    {
        public ImageFormatException() { }

        public ImageFormatException(string message) : base(message) { }

        public ImageFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}
