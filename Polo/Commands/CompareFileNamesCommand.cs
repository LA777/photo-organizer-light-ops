﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Comparers;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class CompareFileNamesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter()
        };

        public CompareFileNamesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => "compare-file-names";

        public string ShortName => "cfn";

        public string Description => $"Compares all files by file names only from the source (current) and destination directory.";

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var destinationFolder = ParameterHandler.DestinationParameter.Initialize(parameters, null);

            var allFilesSourceFolder = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly).ToList();
            var allFilesDestinationFolder = Directory.EnumerateFiles(destinationFolder, "*.*", SearchOption.TopDirectoryOnly).ToList();

            var fileNameComparer = new FileNameComparer();
            var differenceSourceFolder = allFilesSourceFolder.Except(allFilesDestinationFolder, fileNameComparer);
            var differenceDestinationFolder = allFilesDestinationFolder.Except(allFilesSourceFolder, fileNameComparer);

            if (!differenceSourceFolder.Any() && !differenceDestinationFolder.Any())
            {
                _logger.Information($"Folders are equal.");

                return;
            }

            ShowFolderDifferenceFiles(sourceFolder, differenceSourceFolder);
            ShowFolderDifferenceFiles(destinationFolder, differenceDestinationFolder);
        }

        private void ShowFolderDifferenceFiles(string folderFullPath, IEnumerable<string> differenceFiles)
        {
            if (differenceFiles.Any())
            {
                _logger.Information($"Folder path: {folderFullPath}");

                foreach (var file in differenceFiles)
                {
                    _logger.Information($"File: {Path.GetFileName(file)}");
                }
            }
        }
    }
}