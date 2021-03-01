using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class MoveVideoToSubfolderCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly string VideoSubfolderName = "video";

        public string Name => "video";

        public string ShortName => "v";

        public string Description => "Creates Video sub-folder in the current folder and moves all video files to this sub-folder.";

        public MoveVideoToSubfolderCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var videoFolderPath = Path.Join(currentDirectory, VideoSubfolderName);
            Directory.CreateDirectory(videoFolderPath);

            var videoFiles = new List<string>();

            _applicationSettings.VideoFileExtensions.ToList()
                .ForEach(x => videoFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));

            foreach (var filePath in videoFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var destinationFilePath = Path.Join(fileInfo.DirectoryName, VideoSubfolderName, fileInfo.Name);
                File.Move(filePath, destinationFilePath);
                _logger.Information($"Moved: {fileInfo.Name}");
            }
        }
    }
}
