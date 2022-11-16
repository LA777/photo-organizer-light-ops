using Polo.Abstractions.Wrappers;

namespace Polo.Wrappers
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}