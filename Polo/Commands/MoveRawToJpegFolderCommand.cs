﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Comparers;
using Serilog;

namespace Polo.Commands
{
    public class MoveRawToJpegFolderCommand : ICommand
    {
        public const string NameLong = "move-raw";
        public const string NameShort = "mr";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public MoveRawToJpegFolderCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Move RAW files to the RAW sub-folder in the JPEG folder.";

        public IParameterHandler ParameterHandler { get; } = null!;

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            // TODO LA - Cover new logic with UTs
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName); // TODO LA - Create RAW input parameter

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(rawFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));

            var jpegFilesInCurrentFolder = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => jpegFilesInCurrentFolder.AddRange(Directory.EnumerateFiles(currentDirectory, $"*{x}", SearchOption.TopDirectoryOnly)));

            var fileNameWithoutExtensionComparer = new FileNameWithoutExtensionComparer(); // TODO LA - Use Comparer via DI
            var orphanageRawFiles = rawFiles.Except(jpegFilesInCurrentFolder, fileNameWithoutExtensionComparer).ToList();

            var parentFolder = Directory.GetParent(currentDirectory)!.FullName;
            var jpegFilesInParentFolder = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => jpegFilesInParentFolder.AddRange(Directory.EnumerateFiles(parentFolder, $"*{x}", SearchOption.AllDirectories)));
            var jpegFilesRelatedToOrphanageRawFiles = jpegFilesInParentFolder.Intersect(orphanageRawFiles, fileNameWithoutExtensionComparer).ToList();

            var index = 0;
            foreach (var jpegFile in jpegFilesRelatedToOrphanageRawFiles)
            {
                var jpegFolder = Directory.GetParent(jpegFile)!.FullName;

                var rawSubfolderPath = Path.Combine(jpegFolder, _applicationSettings.RawFolderName);
                if (!Directory.Exists(rawSubfolderPath))
                {
                    Directory.CreateDirectory(rawSubfolderPath);
                }

                var rawFilesForProcess = orphanageRawFiles.Where(x => fileNameWithoutExtensionComparer.Equals(x, jpegFile)).Select(x => x);

                foreach (var rawFileForProcess in rawFilesForProcess)
                {
                    var rawFileName = Path.GetFileName(rawFileForProcess);
                    var destinationRawFilePath = Path.Combine(rawSubfolderPath, rawFileName);

                    File.Move(rawFileForProcess, destinationRawFilePath);
                    _logger.Information($"[{++index}/{jpegFilesRelatedToOrphanageRawFiles.Count}] Moved: {rawFileName} to '{Path.Combine(jpegFolder, _applicationSettings.RawFolderName)}'");
                }
            }
        }
    }
}