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
    public class ConvertExifTimezoneCommand : ICommand
    {
        public const string NameLong = "convert-exif-timezone";
        public const string NameShort = "cet";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public ConvertExifTimezoneCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Convert EXIF from one timezone to another.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            TimeDifferenceParameter = new TimeDifferenceParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            // TODO LA - Cover with UTs
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter!.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var destinationFolder = Path.GetFullPath(outputFolderName, sourceFolderPath);
            var timezoneTimeDifference = ParameterHandler.TimeDifferenceParameter!.Initialize(parameters, 0);

            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            if (imagesForProcess.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
                _logger.Information($"Destination directory created: {destinationFolder}");
            }

            foreach (var imageForProcess in imagesForProcess)
            {
                var fileName = Path.GetFileName(imageForProcess);
                var destinationImagePath = Path.Combine(destinationFolder, fileName);

                using var image = new MagickImage(imageForProcess);
                var exifProfile = image.GetExifProfile();
                //DateTime fileDateModified;
                if (exifProfile == null)
                {
                    exifProfile = new ExifProfile();
                    //var fileDateModified = File.GetLastWriteTime(imageForProcess);
                }

                var exifDateTimeOriginal = exifProfile.GetValue(ExifTag.DateTimeOriginal);
                //var exifDateTime = exifProfile.GetValue(ExifTag.DateTime);
                //var exifDateTimeDigitized = exifProfile.GetValue(ExifTag.DateTimeDigitized);

                var currentTime = DateTime.ParseExact(exifDateTimeOriginal!.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                var updatedTime = currentTime.AddHours(timezoneTimeDifference);

                //fileDateModified = Convert.ToDateTime(exifDateTime);

                var updatedTimeFormatted = updatedTime.ToString("yyyy:MM:dd HH:mm:ss");

                exifProfile.SetValue(ExifTag.DateTimeOriginal, updatedTimeFormatted);
                exifProfile.SetValue(ExifTag.DateTime, updatedTimeFormatted);
                exifProfile.SetValue(ExifTag.DateTimeDigitized, updatedTimeFormatted);

                image.SetProfile(exifProfile);
                image.Write(destinationImagePath);

                _logger.Information($"File copied with updated EXIF: {destinationImagePath}");
            }
        }
    }
}