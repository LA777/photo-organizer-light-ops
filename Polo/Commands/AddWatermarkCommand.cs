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
    public class AddWatermarkCommand : ICommand
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
            ImageQuality = new ImageQuality()
        };

        public string Name => "add-watermark";

        public string ShortName => "aw";

        public string Description => $"Adds watermarks to all JPG files and copies to the output folder. Example: polo.exe {CommandParser.CommandPrefix}{Name}";

        public AddWatermarkCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Add OverwriteFile parameter

            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var watermarkPath = ParameterHandler.WatermarkPathParameter.Initialize(parameters, _applicationSettings.WatermarkPath);
            var watermarkOutputFolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.WatermarkOutputFolderName);
            var destinationDirectory = Path.Combine(sourceFolderPath, watermarkOutputFolderName);
            var watermarkPosition = ParameterHandler.PositionParameter.Initialize(parameters, _applicationSettings.WatermarkPosition);
            var watermarkPositionMagick = watermarkPosition.ParsePosition();
            var watermarkTransparencyPercent = ParameterHandler.TransparencyParameter.Initialize(parameters, _applicationSettings.WatermarkTransparencyPercent);
            var imageQuality = ParameterHandler.ImageQuality.Initialize(parameters, _applicationSettings.ImageQuality);

            // TODO LA - Check in UTs duplicates
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()// TODO LA - Move this Select to some extension
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*.{x}", SearchOption.TopDirectoryOnly)));

            if (imagesForProcess.Any() && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            using var watermark = new MagickImage(watermarkPath);
            using var transparentWatermark = watermark.ConvertToTransparentMagickImage(watermarkTransparencyPercent);

            foreach (var imageForProcess in imagesForProcess)
            {
                var fileName = Path.GetFileName(imageForProcess);
                var destinationImagePath = Path.Combine(destinationDirectory, fileName);
                using var image = new MagickImage(imageForProcess);
                image.Composite(transparentWatermark, watermarkPositionMagick, CompositeOperator.Over);
                image.Quality = imageQuality; // TODO LA - Cover with UTs
                image.Write(destinationImagePath);
                _logger.Information($"Watermark added: {destinationImagePath}");
            }
        }
    }
}
