using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class MoveVideoToSubfolderCommand : ICommand
    {
        public const string NameLong = "move-video";
        public const string NameShort = "mv";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;
        private readonly string VideoSubfolderName = "video"; // TODO LA - Create Parameter and Setting for Video folder name

        public MoveVideoToSubfolderCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Creates Video sub-folder in the current folder and moves all video files to this sub-folder.";

        public IParameterHandler ParameterHandler { get; }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var videoFolderPath = Path.Join(currentDirectory, VideoSubfolderName); // TODO LA - Use parameter VideoSubfolderName
            Directory.CreateDirectory(videoFolderPath);

            var videoFiles = new List<string>();

            _applicationSettings.VideoFileExtensions.Distinct().ToList()
                .ForEach(x => videoFiles.AddRange(Directory.EnumerateFiles(currentDirectory, $"*{x}", SearchOption.TopDirectoryOnly))); // TODO LA - Refactor

            foreach (var filePath in videoFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var destinationFilePath = Path.Join(fileInfo.DirectoryName, VideoSubfolderName, fileInfo.Name);
                File.Move(filePath, destinationFilePath);
                _logger.Information($"Moved: {fileInfo.Name} to '{VideoSubfolderName}'");
            }
        }
    }
}