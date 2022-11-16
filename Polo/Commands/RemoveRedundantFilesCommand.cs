using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using SearchOption = System.IO.SearchOption;

namespace Polo.Commands
{
    public class RemoveRedundantFilesCommand : ICommand
    {
        public const string NameLong = "remove-redundant-files";
        public const string NameShort = "rrf";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public RemoveRedundantFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Removes redundant files from the folder and sub folders.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            // TODO LA - Cover with UTs
            // TODO LA - Add RecursiveParameter
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);

            // TODO LA - Cover with UTs - check for duplicates
            // TODO LA - add Parameter for RedundantFiles names
            var redundantFiles = new List<string>();
            _applicationSettings.RedundantFiles.Distinct().ToList()
                .ForEach(x => redundantFiles.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"{x}", SearchOption.AllDirectories))); // TODO LA - Refactor

            foreach (var redundantFile in redundantFiles)
            {
                FileSystem.DeleteFile(redundantFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                _logger.Information($"File deleted: {redundantFile}");
            }
        }
    }
}