﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class RemoveOrphanageRawCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public string Name => "remove-orphanage-raw";

        public string ShortName => "ror";

        public string Description => "Removes orphanage raw files from the RAW folder.";

        public RemoveOrphanageRawCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName);

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(rawFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            var orphanageRawFiles = new List<string>();

            foreach (var rawFile in rawFiles)
            {
                var rawFileInfo = new FileInfo(rawFile);
                var rawFileShortName = Path.GetFileNameWithoutExtension(rawFileInfo.Name);

                var jpegFiles = new List<string>();
                _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                    .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"{rawFileShortName}.{x}", SearchOption.TopDirectoryOnly)));

                if (!jpegFiles.Any())
                {
                    orphanageRawFiles.Add(rawFile);
                }
            }

            foreach (var rawFile in orphanageRawFiles)
            {
                var fileInfo = new FileInfo(rawFile);
                var isDeleted = fileInfo.DeleteToRecycleBin();

                _logger.Information(isDeleted
                    ? $"RAW file deleted: {fileInfo.Name}"
                    : $"RAW file not found: {fileInfo.Name}");
            }
        }
    }
}
