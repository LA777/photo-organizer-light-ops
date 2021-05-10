using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polo.Commands
{
    public class RemoveThumbsDbCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
        };

        public string Name => "remove-thumbs-db";

        public string ShortName => "rt";

        public string Description => "Removes thumbs.db files from the folder and sub folders.";

        public RemoveThumbsDbCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);

            var thumbsDbFiles = Directory.EnumerateFiles(sourceFolderPath, "thumbs.db", System.IO.SearchOption.AllDirectories);

            foreach (var thumbsDbFile in thumbsDbFiles)
            {
                FileSystem.DeleteFile(thumbsDbFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                _logger.Information($"Thumbs.db file deleted: {thumbsDbFile}");
            }
        }
    }
}
