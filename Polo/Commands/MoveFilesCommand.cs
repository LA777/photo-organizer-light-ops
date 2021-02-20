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
    public class MoveFilesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IOptions<ApplicationSettings> _applicationOptions;

        public readonly string SourceFolderArgumentName = "source";
        public readonly string DestinationFolderArgumentName = "destination";

        public MoveFilesCommand(IOptions<ApplicationSettings> applicationOptions, ILogger logger)
        {
            _applicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => "move-all-files";

        public string ShortName => "maf";

        public string Description => $"Move all files from source folder to the current folder. Setup path for the source folder in settings or in argument. Setup path to the destination folder in the argument. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{SourceFolderArgumentName}:'e:\\\\DCIM' {CommandParser.ShortCommandPrefix}{DestinationFolderArgumentName}:'c:\\\\photo'";

        public void Action(IReadOnlyDictionary<string, string> arguments = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = _applicationOptions.Value.DefaultSourceDriveName;
            var destinationDirectory = Environment.CurrentDirectory;

            if ((arguments == null || !arguments.Any()) & string.IsNullOrWhiteSpace(sourceFolder)) // TODO LA - Check this with tests
            {
                _logger.Information("Please provide additional arguments or setup settings.");

                return;
            }

            if (arguments != null && arguments.Any()) // TODO LA - Check this with tests
            {
                if (arguments.TryGetValue(SourceFolderArgumentName, out string sourceFolderPath))
                {
                    sourceFolder = sourceFolderPath;
                }

                if (arguments.TryGetValue(DestinationFolderArgumentName, out string destinationFolderPath))
                {
                    destinationDirectory = destinationFolderPath;
                }
            }

            //if (arguments.Length == 2)
            //{
            //    sourceFolder = arguments[1];
            //}
            //else if (arguments.Length == 3)
            //{
            //    sourceFolder = arguments[1];
            //    destinationDirectory = arguments[2];
            //}

            if (!Directory.Exists(sourceFolder))
            {
                throw new DirectoryNotFoundException($"Directory {sourceFolder} does not exists.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"Directory {destinationDirectory} does not exists.");
            }

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationDirectory, destinationFileName);
                File.Move(file, destinationFilePath, false);
                _logger.Information($"File moved: {destinationFileName}");
            }
        }
    }
}
