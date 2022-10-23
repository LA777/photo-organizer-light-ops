using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class ShowVideoFilesCommand : ICommand
    {
        public const string NameLong = "show-video-files";
        public const string NameShort = "svf";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public ShowVideoFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Shows full paths of the all video files in all subfolders.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter()
        };


        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            // TODO LA - Cover with UTs
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
            var extensionsUpper = _applicationSettings.VideoFileExtensions.Distinct().Select(x => x.ToUpper());

            var videoFiles = Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories)
                .Where(x => extensionsUpper.Contains(Path.GetExtension(x).ToUpper()))
                .ToList();

            _logger.Information($"Files found: {videoFiles.Count}");

            foreach (var videoFile in videoFiles)
            {
                _logger.Information(videoFile);
            }
        }
    }
}