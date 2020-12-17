using Polo.Abstractions.Services;
using System;

namespace Polo.Services
{
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
