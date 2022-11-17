using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class DeleteFilesByExtensionCommand : ICommand
    {
        public const string NameLong = "delete-files-by-extension";
        public const string NameShort = "dfbe";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public DeleteFilesByExtensionCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Permanently deletes all files with specified extension.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            RecursiveParameter = new RecursiveParameter(),
            ExtensionParameter = new ExtensionParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, _applicationSettings.DefaultSourceFolderPath);
            var extension = ParameterHandler.ExtensionParameter.Initialize(parameters, null!);
            var isRecursive = ParameterHandler.RecursiveParameter.Initialize(parameters, true);
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var allFiles = Directory.EnumerateFiles(sourceFolder, $"*.{extension}", searchOption).ToList();
            var filesCount = allFiles.Count;

            foreach (var (fileFullPath, index) in allFiles.WithIndex())
            {
                File.Delete(fileFullPath);
                _logger.Information($"[{index}/{filesCount}] File was deleted: {fileFullPath}");
            }
        }
    }
}