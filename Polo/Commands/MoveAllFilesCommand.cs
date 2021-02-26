﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Extensions;
using Polo.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polo.Commands
{
    public class MoveAllFilesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IOptions<ApplicationSettings> _applicationOptions;
        public static readonly string SourceFolderParameterName = "source";
        public static readonly string DestinationFolderParameterName = "destination";

        public MoveAllFilesCommand(IOptions<ApplicationSettings> applicationOptions, ILogger logger)
        {
            _applicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => "move-all-files";

        public string ShortName => "maf";

        public string Description => $"Move all files from source folder to the current folder. Setup path for the source folder in settings or in the parameter. Setup path to the destination folder in the parameter. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{SourceFolderParameterName}:'e:\\\\DCIM' {CommandParser.ShortCommandPrefix}{DestinationFolderParameterName}:'c:\\\\photo'";

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = _applicationOptions.Value.DefaultSourceDriveName;
            var destinationDirectory = Environment.CurrentDirectory;
            var parametersEmpty = parameters.IsNullOrEmpty(); // TODO LA - Check this with tests

            if (parametersEmpty && string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{SourceFolderParameterName}' parameters or setup setting value '{nameof(ApplicationSettings.DefaultSourceDriveName)}'.");
            }

            if (!parametersEmpty) // TODO LA - Check this with tests
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
                throw new DirectoryNotFoundException($"ERROR: Directory '{sourceFolder}' does not exists.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"ERROR: Directory '{destinationDirectory}' does not exists.");
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
