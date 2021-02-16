﻿using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Options;
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
        private readonly ApplicationSettings _applicationSettings;

        public string Name => "resize";

        public string ShortName => "rs";

        public string Description => "Resizes all JPEG images in the current folder and saves them to a sub-folder.";

        public ResizeCommand(IOptions<ApplicationSettings> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - add arguments

            var currentDirectory = Environment.CurrentDirectory;
            var destinationDirectory = Path.Combine(currentDirectory, _applicationSettings.ResizedImageSubfolderName);

            var jpegFiles = new List<string>();
            _applicationSettings.JpegFileExtensions.Distinct().ToList()
                .ForEach(x => jpegFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));

            if (jpegFiles.Any() && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            foreach (var jpegFile in jpegFiles)
            {
                var jpegFileInfo = new FileInfo(jpegFile);
                var destinationImagePath = Path.Combine(destinationDirectory, jpegFileInfo.Name);

                using var image = new MagickImage(jpegFile);
                var info = new MagickImageInfo(jpegFile);

                var width = info.Width;
                var height = info.Height;
                var sizeLimit = _applicationSettings.ImageResizeLongSide;

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
                    File.Copy(jpegFile, destinationImagePath);
                    _logger.Information($"File copied without resize: {destinationImagePath}");

                    continue;
                }

                image.Write(destinationImagePath);
                _logger.Information($"File resized: {destinationImagePath}");
            }
        }
    }
}