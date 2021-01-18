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
    public class MoveVideoToSubfolderCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        private readonly ApplicationSettings _applicationSettings;
        private readonly string VideoSubfolderName = "video";

        public MoveVideoToSubfolderCommand(IOptions<ApplicationSettings> applicationOptions, IConsoleService consoleService)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public string Name => "video";

        public string ShortName => "v";

        public string Description => "Creates Video sub-folder in the current folder and moves all video files to this sub-folder.";

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var videoFolderPath = Path.Join(currentDirectory, VideoSubfolderName);
            Directory.CreateDirectory(videoFolderPath);

            List<string> videoFiles = new List<string>();

            _applicationSettings.VideoFileExtensions.ToList()
                .ForEach(x => videoFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));

            foreach (var filePath in videoFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var destinationFilePath = Path.Join(fileInfo.DirectoryName, VideoSubfolderName, fileInfo.Name);
                File.Move(filePath, destinationFilePath);
                _consoleService.WriteLine($"Moved: {fileInfo.Name}");
            }
        }
    }
}
