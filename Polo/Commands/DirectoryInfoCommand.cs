using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class DirectoryInfoCommand : ICommand
    {
        public const string NameLong = "directory-info";
        public const string NameShort = "di";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public DirectoryInfoCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Shows information about size of subdirectories.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, _applicationSettings.DefaultSourceFolderPath);
            var subFolders = Directory.EnumerateDirectories(sourceFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var subFolder in subFolders)
            {
                var subFolderInfo = new DirectoryInfo(subFolder);
                var folderSize = DirectorySize(subFolderInfo);
                Console.WriteLine($"{subFolderInfo.Name}\t\t\t{folderSize:##,#}");
            }
        }

        private static long DirectorySize(DirectoryInfo directoryInfo)
        {
            // Add file sizes.
            var fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Sum(fileInfo => fileInfo.Length);
            // Add sub-directory sizes.
            var directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Sum(DirectorySize);
            return size;
        }
    }
}