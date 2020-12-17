using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class CopyFilesCommand : ICommand
    {
        private readonly IOptions<ApplicationSettings> _applicationOptions;

        public CopyFilesCommand(IOptions<ApplicationSettings> applicationOptions)
        {
            _applicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        public string Name => "copy";

        public string ShortName => "c";

        public string Description => "Copy files.";

        public void Action(string[] arguments = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = _applicationOptions.Value.DefaultSourceDriveName;
            var destinationDirectory = Environment.CurrentDirectory;

            if ((arguments == null || arguments.Length == 1) & string.IsNullOrWhiteSpace(sourceFolder))
            {
                Console.WriteLine("Please provide additional arguments.");

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

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly).ToList();

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationDirectory, destinationFileName);
                File.Copy(file, destinationFilePath);
                Console.WriteLine($"File copied: {destinationFileName}");
            }
        }
    }
}
