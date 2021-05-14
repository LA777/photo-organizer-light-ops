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
    public class ResizeCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            LongSideLimitParameter = new LongSideLimitParameter(),
            MegaPixelsLimitParameter = new MegaPixelsLimitParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter(),
            ImageQuality = new ImageQuality()
        };

        public string Name => "resize";

        public string ShortName => "rs";

        public string Description => $"Resizes all JPEG images in the current folder and saves them to a sub-folder. Example: polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{LongSideLimitParameter.Name}:1600";

        public ResizeCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.ResizedImageSubfolderName);
            var destinationFolder = Path.Combine(sourceFolderPath, outputFolderName);
            var sizeLimit = ParameterHandler.LongSideLimitParameter.Initialize(parameters, _applicationSettings.ImageResizeLongSideLimit);
            var megaPixelsLimit = ParameterHandler.MegaPixelsLimitParameter.Initialize(parameters, _applicationSettings.ImageResizeMegaPixelsLimit);
            var imageQuality = ParameterHandler.ImageQuality.Initialize(parameters, _applicationSettings.ImageQuality);

            var jpegFiles = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));
            jpegFiles.SortByFileName();

            if (jpegFiles.Any() && !Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            foreach (var jpegFile in jpegFiles)
            {
                var jpegFileInfo = new FileInfo(jpegFile);
                var destinationImagePath = Path.Combine(destinationFolder, jpegFileInfo.Name);

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
                else
                {
                    var imageResolution = width * height;
                    if (imageResolution > (megaPixelsLimit * 1000000))
                    {
                        image.ResizeByMegapixelsLimit(megaPixelsLimit, magickImageInfo);
                    }

                    File.Copy(jpegFile, destinationImagePath);
                    _logger.Information($"File copied without resize: {destinationImagePath}");

                    continue;
                }

                image.Quality = imageQuality; // TODO LA - Cover with UTs
                image.Write(destinationImagePath);
                _logger.Information($"File resized: {destinationImagePath}");
            }
        }
    }
}
