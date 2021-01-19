using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class RemoveOrphanageRawCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        private readonly ApplicationSettings _applicationSettings;

        public string Name => "remove-orphanage-raw";

        public string ShortName => "ror";

        public string Description => "Removes orphanage raw files from the RAW folder.";

        public RemoveOrphanageRawCommand(IOptions<ApplicationSettings> applicationOptions, IConsoleService consoleService)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName);

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(rawFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            var orphanageRawFiles = new List<string>();

            foreach (var rawFile in rawFiles)
            {
                var rawFileInfo = new FileInfo(rawFile);
                var rawFileShortName = Path.GetFileNameWithoutExtension(rawFileInfo.Name);

                var jpegFiles = new List<string>();
                _applicationSettings.JpegFileExtensions.ToList()
                    .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"{rawFileShortName}.{x}", SearchOption.TopDirectoryOnly)));

                if (!jpegFiles.Any())
                {
                    orphanageRawFiles.Add(rawFile);
                }
            }

            foreach (var rawFile in orphanageRawFiles)
            {
                var fileInfo = new FileInfo(rawFile);
                fileInfo.Delete();
                _consoleService.WriteLine($"RAW file deleted: {fileInfo.Name}");
            }
        }
    }
}
