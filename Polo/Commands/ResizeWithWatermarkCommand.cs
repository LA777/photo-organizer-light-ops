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
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class ResizeWithWatermarkCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            WatermarkPathParameter = new WatermarkPathParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            PositionParameter = new PositionParameter(),
            TransparencyParameter = new TransparencyParameter(),
            LongSideLimitParameter = new LongSideLimitParameter()
        };

        public string Name => "resize-with-watermark";

        public string ShortName => "rww";

        public string Description => $"Resizes all JPEG images in the current folder, adds watermark and saves them to a sub-folder. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{LongSideLimitParameter.Name}:1600"; // TODO LA - Show all possible parameters

        public ResizeWithWatermarkCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Add OverwriteFile parameter and setting

            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var watermarkPath = ParameterHandler.WatermarkPathParameter.Initialize(parameters, _applicationSettings.WatermarkPath);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.WatermarkOutputFolderName);
            var destinationFolder = Path.Combine(sourceFolderPath, outputFolderName);
            var sizeLimit = ParameterHandler.LongSideLimitParameter.Initialize(parameters, _applicationSettings.ImageResizeLongSideLimit);
            var watermarkPosition = ParameterHandler.PositionParameter.Initialize(parameters, _applicationSettings.WatermarkPosition);
            var watermarkPositionMagick = watermarkPosition.ParsePosition();
            var watermarkTransparencyPercent = ParameterHandler.TransparencyParameter.Initialize(parameters, _applicationSettings.WatermarkTransparencyPercent);

            // TODO LA - Check in UTs duplicates
            var jpegFiles = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()// TODO LA - Move this Select to some extension
                .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            if (jpegFiles.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            using var watermark = new MagickImage(watermarkPath);
            using var transparentWatermark = watermark.ConvertToTransparentMagickImage(watermarkTransparencyPercent);

            foreach (var jpegFile in jpegFiles)
            {
                var fileName = Path.GetFileName(jpegFile);
                var destinationImagePath = Path.Combine(destinationFolder, fileName);

                using var image = new MagickImage(jpegFile);
                var info = new MagickImageInfo(jpegFile);
                var width = info.Width;
                var height = info.Height;

                if (width >= height && width > sizeLimit)
                {
                    image.Resize(sizeLimit, 0);
                }
                else if (height > width && height > sizeLimit)
                {
                    image.Resize(0, sizeLimit);
                }
                else
                {
                    image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                    image.Write(destinationImagePath);
                    _logger.Information($"File copied with watermark and without resize: {destinationImagePath}");

                    continue;
                }

                image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                image.Write(destinationImagePath);
                _logger.Information($"File resized with watermark: {destinationImagePath}");
            }
        }
    }
}
