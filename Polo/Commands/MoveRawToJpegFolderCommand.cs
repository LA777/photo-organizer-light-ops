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
    public class MoveRawToJpegFolderCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;

        public string Name => "move-raw";

        public string ShortName => "mr";

        public string Description => "Move RAW files to the RAW sub-folder in the JPEG folder.";

        public MoveRawToJpegFolderCommand(IOptions<ApplicationSettings> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName);

            var rawFiles = new List<string>();

            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(rawFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            foreach (var rawFilePath in rawFiles)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rawFilePath);
                var allFotosFolder = Directory.GetParent(currentDirectory).FullName;

                var jpegFiles = new List<string>();
                _applicationSettings.JpegFileExtensions.Distinct().ToList()
                    .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(allFotosFolder, $"{fileNameWithoutExtension}.{x}", SearchOption.AllDirectories)));

                if (jpegFiles.Count() > 1)
                {
                    throw new Exception($"Files more than one. {jpegFiles.First()}");
                }

                foreach (var jpegFile in jpegFiles)
                {
                    var parentFolderFullPath = Directory.GetParent(jpegFile).FullName;
                    var rawSubFolderPath = Path.Combine(parentFolderFullPath, _applicationSettings.RawFolderName);
                    Directory.CreateDirectory(rawSubFolderPath);

                    var fileInfo = new FileInfo(rawFilePath);
                    var destinationFilePath = Path.Join(rawSubFolderPath, fileInfo.Name);
                    fileInfo.MoveTo(destinationFilePath);
                    _logger.Information($"Moved: {fileInfo.Name}");
                }
            }
        }
    }
}
