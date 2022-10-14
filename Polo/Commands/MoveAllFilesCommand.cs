﻿using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class MoveAllFilesCommand : ICommand
    {
        public const string NameLong = "move-all-files";
        public const string NameShort = "maf";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public MoveAllFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Move all files from source folder to the current folder. Setup path for the source folder in settings or in the parameter. Setup path to the destination folder in the parameter.";

        public string Example => "polo.exe {CommandParser.CommandPrefix}{Name} {CommandParser.ShortCommandPrefix}{ParameterHandler.SourceParameter.Name}:'e:\\DCIM' {CommandParser.ShortCommandPrefix}{ParameterHandler.DestinationParameter.Name}:'c:\\photo'"; // TODO LA

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, _applicationSettings.DefaultSourceFolderPath);
            var destinationFolder = ParameterHandler.DestinationParameter.Initialize(parameters, Environment.CurrentDirectory);

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationFolder, destinationFileName);
                File.Move(file, destinationFilePath, false);
                _logger.Information($"File moved: {destinationFileName}");
            }
        }
    }
}