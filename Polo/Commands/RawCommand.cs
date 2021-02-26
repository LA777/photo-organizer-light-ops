using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class RawCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;

        public string Name => "raw";

        public string ShortName => "r";

        public string Description => "Creates RAW sub-folder in the current folder and moves all RAW files to this sub-folder.";

        public RawCommand(IOptions<ApplicationSettings> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName);
            Directory.CreateDirectory(rawFolderPath);

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));

            foreach (var rawFilePath in rawFiles)
            {
                var fileInfo = new FileInfo(rawFilePath);
                var destinationFilePath = Path.Join(rawFolderPath, fileInfo.Name);
                File.Move(rawFilePath, destinationFilePath);
                _logger.Information($"Moved: {fileInfo.Name}");
            }
        }
    }
}
