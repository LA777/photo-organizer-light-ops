﻿using Polo.Commands;
using System.Collections.Generic;

namespace Polo
{
    public class SupportedCommands
    {
        private static readonly IEnumerable<ICommand> Commands = new List<ICommand>
        {
            new VersionCommand(),
            new HelpCommand(),
            new RawCommand()
        };

        public static IEnumerable<ICommand> GetCommandsList()
        {
            return Commands;
        }
    }
}