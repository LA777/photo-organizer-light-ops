﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Comparers;
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
            // TODO LA - Cover new logic with UTs
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName);// TODO LA - Add RawFolder parameter

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(rawFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));

            var jpegFilesInCurrentFolder = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => jpegFilesInCurrentFolder.AddRange(Directory.EnumerateFiles(currentDirectory, $"*{x}", SearchOption.TopDirectoryOnly)));

            var fileNameWithoutExtensionComparer = new FileNameWithoutExtensionComparer();// TODO LA - Use Comparer via DI
            var orphanageRawFiles = rawFiles.Except(jpegFilesInCurrentFolder, fileNameWithoutExtensionComparer);

            var parentFolder = Directory.GetParent(currentDirectory).FullName;
            var jpegFilesInParentFolder = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
            .ForEach(x => jpegFilesInParentFolder.AddRange(Directory.EnumerateFiles(parentFolder, $"*{x}", SearchOption.AllDirectories)));

            var orphanageRawFilesToDelete = orphanageRawFiles.Except(jpegFilesInParentFolder, fileNameWithoutExtensionComparer);

            int index = 0;
            foreach (var rawFile in orphanageRawFilesToDelete)
            {
                var fileInfo = new FileInfo(rawFile);
                var isDeleted = fileInfo.DeleteToRecycleBin();

                _logger.Information(isDeleted
                    ? $"[{++index}/{orphanageRawFilesToDelete.Count()}] RAW file deleted: {fileInfo.Name}"
                    : $"RAW file not found: {fileInfo.Name}");
            }
        }
    }
}
