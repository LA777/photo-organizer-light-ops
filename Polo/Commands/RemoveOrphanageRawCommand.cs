using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class RemoveOrphanageRawCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        public string Name => "remove-orphanage-raw";

        public string ShortName => "ror";

        public string Description => "Removes orphanage raw files from RAW folder.";

        public RemoveOrphanageRawCommand(IConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var jpegFiles = Directory.EnumerateFiles(currentDirectory, "*.JPG", SearchOption.TopDirectoryOnly).ToList(); // TODO - use settings file

            var rawFolderPath = Path.Join(currentDirectory, "RAW");
            var rawFiles = Directory.EnumerateFiles(rawFolderPath, "*.ORF", SearchOption.TopDirectoryOnly).ToList(); // TODO - use settings file

            var jpegFilesNames = jpegFiles.Select(Path.GetFileNameWithoutExtension);
            var rawFilesNames = rawFiles.Select(Path.GetFileNameWithoutExtension);

            var orphanageRawFilesNames = rawFilesNames.Except(jpegFilesNames);
            var orphanageRawFiles = orphanageRawFilesNames.Select(x => Path.Join(rawFolderPath, $"{x}.ORF")); // TODO - use settings file

            foreach (var rawFile in orphanageRawFiles)
            {
                File.Delete(rawFile);
                var fileInfo = new FileInfo(rawFile);
                _consoleService.WriteLine($"RAW file deleted: {fileInfo.Name}");
            }
        }
    }
}
