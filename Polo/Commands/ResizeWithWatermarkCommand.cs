using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class ResizeWithWatermarkCommand : ICommand
    {
        public const string NameLong = "resize-with-watermark";
        public const string NameShort = "rww";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public ResizeWithWatermarkCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Resizes all JPEG images in the current folder, adds watermark and saves them to a sub-folder."; // TODO LA - Show all possible parameters

        public string Example => "polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{ParameterHandler.LongSideLimitParameter.Name}:1600"; // TODO LA

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            WatermarkPathParameter = new WatermarkPathParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            PositionParameter = new PositionParameter(),
            TransparencyParameter = new TransparencyParameter(),
            LongSideLimitParameter = new LongSideLimitParameter(),
            MegaPixelsLimitParameter = new MegaPixelsLimitParameter(),
            ImageQualityParameter = new ImageQualityParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Add OverwriteFile parameter and setting

            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var watermarkPath = ParameterHandler.WatermarkPathParameter.Initialize(parameters, _applicationSettings.WatermarkPath);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var destinationFolder = Path.GetFullPath(outputFolderName, sourceFolderPath);
            var sizeLimit = ParameterHandler.LongSideLimitParameter.Initialize(parameters, _applicationSettings.ImageResizeLongSideLimit);
            var megaPixelsLimit = ParameterHandler.MegaPixelsLimitParameter.Initialize(parameters, _applicationSettings.ImageResizeMegaPixelsLimit);
            var watermarkPosition = ParameterHandler.PositionParameter.Initialize(parameters, _applicationSettings.WatermarkPosition);
            var watermarkPositionMagick = watermarkPosition.ParsePosition();
            var watermarkTransparencyPercent = ParameterHandler.TransparencyParameter.Initialize(parameters, _applicationSettings.WatermarkTransparencyPercent);
            var imageQuality = ParameterHandler.ImageQualityParameter.Initialize(parameters, _applicationSettings.ImageQuality);

            // TODO LA - Check in UTs duplicates
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList() // TODO LA - Move this Select to some extension
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            if (imagesForProcess.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            using var watermark = new MagickImage(watermarkPath);
            using var transparentWatermark = watermark.ConvertToTransparentMagickImage(watermarkTransparencyPercent);

            var index = 0;
            foreach (var jpegFile in imagesForProcess)
            {
                var fileName = Path.GetFileName(jpegFile);
                var destinationImagePath = Path.Combine(destinationFolder, fileName);

                using var image = new MagickImage(jpegFile);
                var magickImageInfo = new MagickImageInfo(jpegFile);
                var width = magickImageInfo.Width;
                var height = magickImageInfo.Height;

                if (width >= height && width > sizeLimit)
                {
                    image.Resize(sizeLimit, 0);
                }
                else if (height > width && height > sizeLimit)
                {
                    image.Resize(0, sizeLimit);
                }
                else if (width * height > megaPixelsLimit * 1000000)
                {
                    image.ResizeByMegapixelsLimit(megaPixelsLimit, magickImageInfo);
                }
                else
                {
                    image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                    image.Quality = imageQuality; // TODO LA - Cover with UTs
                    image.Write(destinationImagePath);
                    _logger.Information($"[{++index}/{imagesForProcess.Count}] File copied with watermark and without resize: {destinationImagePath}");

                    continue;
                }

                image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                image.Quality = imageQuality; // TODO LA - Cover with UTs
                image.Write(destinationImagePath);
                _logger.Information($"[{++index}/{imagesForProcess.Count}] File resized with watermark: {destinationImagePath}");
            }
        }
    }
}