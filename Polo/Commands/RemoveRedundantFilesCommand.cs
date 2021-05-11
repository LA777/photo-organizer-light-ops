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
using System.Linq;

namespace Polo.Commands
{
    public class RemoveRedundantFilesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
        };

        public string Name => "remove-redundant-files";

        public string ShortName => "rrf";

        public string Description => "Removes redundant files from the folder and sub folders.";

        public RemoveRedundantFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);

            // TODO LA - Cover with UTs - check for duplicates
            // TODO LA - add Parameter for RedundatFiles names
            var reduntantFiles = new List<string>();
            _applicationSettings.RedundantFiles.Distinct().ToList()
                .ForEach(x => reduntantFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"{x}", System.IO.SearchOption.AllDirectories)));

            foreach (var reduntantFile in reduntantFiles)
            {
                FileSystem.DeleteFile(reduntantFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                _logger.Information($"File deleted: {reduntantFile}");
            }
        }
    }
}
