﻿using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class UpdateExifDateCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter()
        };

        public string Name => "update-exif-date";

        public string ShortName => "ued";

        public string Description => $"Updates EXIF date and time according to file creation date and time.";

        public UpdateExifDateCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var destinationFolder = Path.GetFullPath(outputFolderName, sourceFolderPath);

            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            if (imagesForProcess.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            foreach (var imageForProcess in imagesForProcess)
            {
                var fileName = Path.GetFileName(imageForProcess);
                var destinationImagePath = Path.Combine(destinationFolder, fileName);

                using var image = new MagickImage(imageForProcess);
                var exifProfile = image.GetExifProfile();
                if (exifProfile == null)
                {
                    exifProfile = new ExifProfile();
                }

                DateTime fileDateModified = File.GetLastWriteTime(imageForProcess);
                var dateFormated = fileDateModified.ToString("yyyy:MM:dd HH:mm:ss");

                exifProfile.SetValue(ExifTag.DateTimeOriginal, dateFormated);
                exifProfile.SetValue(ExifTag.DateTime, dateFormated);
                exifProfile.SetValue(ExifTag.DateTimeDigitized, dateFormated);

                image.SetProfile(exifProfile);
                image.Write(destinationImagePath);

                _logger.Information($"File copied with updated EXIF: {destinationImagePath}");
            }
        }
    }
}
