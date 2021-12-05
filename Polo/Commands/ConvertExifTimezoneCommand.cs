using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class ConvertExifTimezoneCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            TimeDifferenceParameter = new TimeDifferenceParameter()
        };

        public string Name => "convert-exif-timezone";

        public string ShortName => "cetz";

        public string Description => $"Convert EXIF from one timezone to another.";

        public ConvertExifTimezoneCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
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
            var timezonTimeDifference = ParameterHandler.TimeDifferenceParameter.Initialize(parameters);

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
                DateTime fileDateModified;
                if (exifProfile == null)
                {
                    exifProfile = new ExifProfile();
                    fileDateModified = File.GetLastWriteTime(imageForProcess);
                }

                var exifDateTimeOriginal = exifProfile.GetValue(ExifTag.DateTimeOriginal);
                var exifDateTime = exifProfile.GetValue(ExifTag.DateTime);
                var exifDateTimeDigitized = exifProfile.GetValue(ExifTag.DateTimeDigitized);

                var currentTime = DateTime.ParseExact(exifDateTimeOriginal.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                var updatedTime = currentTime.AddHours(timezonTimeDifference);

                //fileDateModified = Convert.ToDateTime(exifDateTime);

                var updatedTimeFormated = updatedTime.ToString("yyyy:MM:dd HH:mm:ss");

                exifProfile.SetValue(ExifTag.DateTimeOriginal, updatedTimeFormated);
                exifProfile.SetValue(ExifTag.DateTime, updatedTimeFormated);
                exifProfile.SetValue(ExifTag.DateTimeDigitized, updatedTimeFormated);

                image.SetProfile(exifProfile);
                image.Write(destinationImagePath);

                _logger.Information($"File copied with updated EXIF: {destinationImagePath}");
            }
        }
    }
}
