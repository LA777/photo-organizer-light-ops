using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class RawCommand : ICommand
    {
        public const string NameLong = "raw";
        public const string NameShort = "r";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public RawCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Creates RAW sub-folder in the current folder and moves all RAW files to this sub-folder.";

        public IParameterHandler ParameterHandler { get; }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, _applicationSettings.RawFolderName); // TODO LA - Add RawFolderName parameter
            Directory.CreateDirectory(rawFolderPath);

            var rawFiles = new List<string>();
            _applicationSettings.RawFileExtensions.Distinct().ToList()
                .ForEach(x => rawFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*{x}", SearchOption.TopDirectoryOnly)));

            foreach (var rawFilePath in rawFiles)
            {
                var fileInfo = new FileInfo(rawFilePath);
                var destinationFilePath = Path.Join(rawFolderPath, fileInfo.Name);
                File.Move(rawFilePath, destinationFilePath);
                _logger.Information($"Moved: {fileInfo.Name} to '{_applicationSettings.RawFolderName}'");
            }
        }
    }
}