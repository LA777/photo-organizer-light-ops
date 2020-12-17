using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polo.Commands
{
    public class RawCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        public string Name => "raw";

        public string ShortName => "r";

        public string Description => "Creates RAW sub-folder in the current folder and moves all RAW files to this sub-folder.";

        public RawCommand(IConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, "RAW");
            Directory.CreateDirectory(rawFolderPath);

            var rawFiles = Directory.EnumerateFiles(currentDirectory, "*.ORF", SearchOption.TopDirectoryOnly);

            foreach (var rawFilePath in rawFiles)
            {
                var fileInfo = new FileInfo(rawFilePath);
                var destinationFilePath = Path.Join(rawFolderPath, fileInfo.Name);
                File.Move(rawFilePath, destinationFilePath);
                _consoleService.WriteLine($"Moved: {fileInfo.Name}");
            }
        }
    }
}
