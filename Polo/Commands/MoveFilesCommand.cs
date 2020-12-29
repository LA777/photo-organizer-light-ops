﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polo.Commands
{
    public class MoveFilesCommand : ICommand
    {
        private readonly IConsoleService _consoleService;
        private readonly IOptions<ApplicationSettings> _applicationOptions;

        public MoveFilesCommand(IOptions<ApplicationSettings> applicationOptions, IConsoleService consoleService)
        {
            _applicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        }

        public string Name => "move";

        public string ShortName => "m";

        public string Description => "Move files.";

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = _applicationOptions.Value.DefaultSourceDriveName;
            var destinationDirectory = Environment.CurrentDirectory;

            if ((arguments == null || arguments.Length == 1) & string.IsNullOrWhiteSpace(sourceFolder))
            {
                _consoleService.WriteLine("Please provide additional arguments.");

                return;
            }

            if (arguments.Length == 2)
            {
                sourceFolder = arguments[1];
            }
            else if (arguments.Length == 3)
            {
                sourceFolder = arguments[1];
                destinationDirectory = arguments[2];
            }

            if (!Directory.Exists(sourceFolder))
            {
                throw new DirectoryNotFoundException($"Directory {sourceFolder} does not exists.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"Directory {destinationDirectory} does not exists.");
            }

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationDirectory, destinationFileName);
                File.Move(file, destinationFilePath, false);
                _consoleService.WriteLine($"File moved: {destinationFileName}");
            }
        }
    }
}
