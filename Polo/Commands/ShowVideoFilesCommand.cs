using Microsoft.Extensions.Options;
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
    public class ShowVideoFilesCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter()
        };

        public ShowVideoFilesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => "show-video-files";

        public string ShortName => "svf";

        public string Description => $"Shows full paths of the all video files in all subfolders.";

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
