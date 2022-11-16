using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class CopyAllFilesCommand : ICommand
    {
        public const string NameLong = "copy-all-files";
        public const string NameShort = "caf";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public CopyAllFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Copy all files from source folder to the current folder. Setup path for the source folder in settings or in argument. Setup path to the destination folder in the argument.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, _applicationSettings.DefaultSourceFolderPath);
            var destinationFolder = ParameterHandler.DestinationParameter!.Initialize(parameters, Environment.CurrentDirectory);
            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                var destinationFileName = Path.GetFileName(file);
                var destinationFilePath = Path.Combine(destinationFolder, destinationFileName);
                File.Copy(file, destinationFilePath);
                _logger.Information($"File copied: {destinationFileName}");
            }
        }
    }
}