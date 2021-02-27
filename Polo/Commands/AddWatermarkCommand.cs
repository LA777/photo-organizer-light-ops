using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Extensions;
using Polo.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class AddWatermarkCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public string Name => "add-watermark";

        public string ShortName => "aw";

        public static readonly string SourceFolderParameterName = "source";
        public static readonly string WatermarkPathParameterName = "watermark-path";
        public static readonly string OutputFolderNameParameterName = "output-folder-name";
        public static readonly string WatermarkPositionParameterName = "position";
        public static readonly string WatermarkTransparencyParameterName = "transparency";

        public string Description => $"Adds watermarks to all JPG files and copies to the output folder. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{WatermarkPathParameterName}:'c:\\watermark.png'";

        public AddWatermarkCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Add overwrite file parameter

            var sourceFolderPath = Environment.CurrentDirectory;

            if (parameters.TryGetValue(SourceFolderParameterName, out var sourceFolderValue))
            {
                sourceFolderPath = sourceFolderValue;

                if (!Directory.Exists(sourceFolderPath))
                {
                    throw new DirectoryNotFoundException($"ERROR: Directory '{sourceFolderPath}' does not exists.");
                }
            }

            var watermarkPath = GetValidStringParameterOrSetting(parameters, WatermarkPathParameterName, _applicationSettings.WatermarkPath);

            if (!File.Exists(watermarkPath))
            {
                throw new FileNotFoundException("ERROR: Watermark file not found.", watermarkPath);
            }

            var watermarkOutputFolderName = GetValidStringParameterOrSetting(parameters, OutputFolderNameParameterName, _applicationSettings.WatermarkOutputFolderName);
            var destinationDirectory = Path.Combine(sourceFolderPath, watermarkOutputFolderName);

            var watermarkPosition = GetValidStringParameterOrSetting(parameters, WatermarkPositionParameterName, _applicationSettings.WatermarkPosition);
            var watermarkPositionMagick = watermarkPosition.ParsePosition();

            var watermarkTransparencyPercent = GetValidIntParameterOrSetting(parameters, WatermarkTransparencyParameterName, _applicationSettings.WatermarkTransparencyPercent, 1, 100);



            // TODO LA - Check in UTs duplicates
            var jpegFiles = new List<string>();
            _applicationSettings.JpegFileExtensions.Distinct().ToList()
                .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            if (jpegFiles.Any() && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            foreach (var jpegFile in jpegFiles)
            {
                var jpegFileInfo = new FileInfo(jpegFile);
                var destinationImagePath = Path.Combine(destinationDirectory, jpegFileInfo.Name);

                using var image = new MagickImage(jpegFile);
                using var watermark = new MagickImage(watermarkPath);
                const int maxPercentValue = 100;
                watermark.Evaluate(Channels.Alpha, EvaluateOperator.Subtract, new Percentage(maxPercentValue - watermarkTransparencyPercent));
                image.Composite(watermark, Gravity.Southwest, CompositeOperator.Over);

                image.Write(destinationImagePath);
                _logger.Information($"Watermark added: {destinationImagePath}");
            }
        }

        private string GetValidStringParameterOrSetting(IReadOnlyDictionary<string, string> parameters, string parameterName, string settingValue)
        {
            var outputValue = parameters.TryGetValue(parameterName, out var parameterValue)
                ? parameterValue : settingValue;

            if (string.IsNullOrEmpty(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{parameterName}' parameter.");
            }

            return outputValue;
        }

        private int GetValidIntParameterOrSetting(IReadOnlyDictionary<string, string> parameters, string parameterName, int settingValue, int min, int max)
        {
            int outputValue;
            if (parameters.TryGetValue(parameterName, out var parameterValue))
            {
                if (int.TryParse(parameterValue, out var number))
                {
                    outputValue = number;

                    if (outputValue < min || outputValue > max)
                    {
                        throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{parameterName}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{parameterName}' should be higher than {min} and smaller than {max}.");
                    }
                }
                else
                {
                    throw new ParseException($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{parameterName}' is not a number.");
                }
            }
            else
            {
                outputValue = settingValue;
            }

            return outputValue;
        }
    }
}
