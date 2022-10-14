using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System.Globalization;

namespace Polo.Commands
{
    public class AddWatermarkWithConvertExifTimezoneCommand : ICommand
    {
        public const string NameLong = "add-watermark-with-convert-exif-timezone";
        public const string NameShort = "awwcet";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public AddWatermarkWithConvertExifTimezoneCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Adds watermarks with converting EXIF from one timezone to another to all JPG files and copies to the output folder.";

        public string Example => "polo.exe {CommandParser.CommandPrefix}{Name}"; // TODO LA

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            WatermarkPathParameter = new WatermarkPathParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            PositionParameter = new PositionParameter(),
            TransparencyParameter = new TransparencyParameter(),
            ImageQualityParameter = new ImageQualityParameter(),
            TimeDifferenceParameter = new TimeDifferenceParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Add OverwriteFile parameter

            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var watermarkPath = ParameterHandler.WatermarkPathParameter.Initialize(parameters, _applicationSettings.WatermarkPath);
            var watermarkOutputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var destinationFolder = Path.GetFullPath(watermarkOutputFolderName, sourceFolderPath); // TODO LA - Add possibility to add full path to Destination directory in settings
            var watermarkPosition = ParameterHandler.PositionParameter.Initialize(parameters, _applicationSettings.WatermarkPosition);
            var watermarkPositionMagick = watermarkPosition.ParsePosition();
            var watermarkTransparencyPercent = ParameterHandler.TransparencyParameter.Initialize(parameters, _applicationSettings.WatermarkTransparencyPercent);
            var imageQuality = ParameterHandler.ImageQualityParameter.Initialize(parameters, _applicationSettings.ImageQuality);
            var timezoneTimeDifference = ParameterHandler.TimeDifferenceParameter.Initialize(parameters, 0);

            _logger.Information("Seeking files...");
            // TODO LA - Check in UTs duplicates
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList() // TODO LA - Move this Select to some extension
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            _logger.Information($"Files for process: {imagesForProcess.Count}");
            imagesForProcess.SortByFileName();

            if (imagesForProcess.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
                _logger.Information($"Destination directory created: {destinationFolder}");
            }

            using var watermark = new MagickImage(watermarkPath);
            using var transparentWatermark = watermark.ConvertToTransparentMagickImage(watermarkTransparencyPercent);

            var index = 0;
            foreach (var imageForProcess in imagesForProcess)
            {
                var fileName = Path.GetFileName(imageForProcess);
                var destinationImagePath = Path.Combine(destinationFolder, fileName);

                using var image = new MagickImage(imageForProcess);

                image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                image.Quality = imageQuality; // TODO LA - Cover with UTs

                // timezone
                var exifProfile = image.GetExifProfile();
                //DateTime fileDateModified;
                if (exifProfile == null)
                {
                    exifProfile = new ExifProfile();
                    //fileDateModified = File.GetLastWriteTime(imageForProcess);
                }

                var exifDateTimeOriginal = exifProfile.GetValue(ExifTag.DateTimeOriginal);
                //var exifDateTime = exifProfile.GetValue(ExifTag.DateTime);
                //var exifDateTimeDigitized = exifProfile.GetValue(ExifTag.DateTimeDigitized);

                var currentTime = DateTime.ParseExact(exifDateTimeOriginal.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                var updatedTime = currentTime.AddHours(timezoneTimeDifference);

                //fileDateModified = Convert.ToDateTime(exifDateTime);

                var updatedTimeFormatted = updatedTime.ToString("yyyy:MM:dd HH:mm:ss");

                exifProfile.SetValue(ExifTag.DateTimeOriginal, updatedTimeFormatted);
                exifProfile.SetValue(ExifTag.DateTime, updatedTimeFormatted);
                exifProfile.SetValue(ExifTag.DateTimeDigitized, updatedTimeFormatted);

                image.SetProfile(exifProfile);
                // timezone end

                image.Write(destinationImagePath);
                _logger.Information($"[{++index}/{imagesForProcess.Count}] Watermark added: {destinationImagePath}");
            }
        }
    }
}