using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polo.Commands
{
    public class CopyAllFilesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IOptions<ApplicationSettingsReadOnly> _applicationOptions;

        public static readonly string SourceFolderParameterName = "source";
        public static readonly string DestinationFolderParameterName = "destination";

        public CopyAllFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => "copy-all-files";

        public string ShortName => "caf";

        public string Description => $"Copy all files from source folder to the current folder. Setup path for the source folder in settings or in argument. Setup path to the destination folder in the argument. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{SourceFolderParameterName}:'e:\\DCIM' {CommandParser.ShortCommandPrefix}{DestinationFolderParameterName}:'c:\\photo'";

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = _applicationOptions.Value.DefaultSourceFolderPath;
            var destinationDirectory = Environment.CurrentDirectory;
            var parametersEmpty = parameters.IsNullOrEmpty();

            if (parametersEmpty && string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{SourceFolderParameterName}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.DefaultSourceFolderPath)}'.");
            }

            if (!parametersEmpty)
            {
                if (parameters.TryGetValue(SourceFolderParameterName, out var sourceFolderPath))
                {
                    sourceFolder = sourceFolderPath;
                }

                if (parameters.TryGetValue(DestinationFolderParameterName, out var destinationFolderPath))
                {
                    destinationDirectory = destinationFolderPath;
                }
            }

            if (!Directory.Exists(sourceFolder))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{sourceFolder}' does not exist.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{destinationDirectory}' does not exist.");
            }

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationDirectory, destinationFileName);
                File.Copy(file, destinationFilePath);
                _logger.Information($"File copied: {destinationFileName}");
            }
        }
    }
}
