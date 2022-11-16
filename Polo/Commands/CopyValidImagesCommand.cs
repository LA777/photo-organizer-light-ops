using ImageMagick;
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
    public class CopyValidImagesCommand : ICommand
    {
        public const string NameLong = "copy-valid-images";
        public const string NameShort = "cvi";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public CopyValidImagesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Copy valid (image contains at least some pixels) images into destination folder.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);
            var destinationFolderPath = ParameterHandler.DestinationParameter!.Initialize(parameters, string.Empty);

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            CopyValidImages(sourceFolderPath, destinationFolderPath);
        }

        private void CopyValidImages(string fullFolderPath, string destinationFolderFullPath)
        {
            var imageFiles = new List<string>();
            _applicationSettings.ImageFileExtensions.Distinct().ToList()
                .ForEach(x => imageFiles.AddRange(Directory.EnumerateFiles(fullFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));

            foreach (var imageFilePath in imageFiles)
            {
                try
                {
                    using var image = new MagickImage(imageFilePath);
                    var imageFileInfo = new FileInfo(imageFilePath);
                    var destinationImagePath = imageFileInfo.GenerateFileFullPath(destinationFolderFullPath);

                    File.Copy(imageFilePath, destinationImagePath);
                    _logger.Information($"Valid file copied: {destinationImagePath}");
                }
                catch (Exception)
                {
                    _logger.Information($"Invalid file skipped: {imageFilePath}");
                }
            }

            var subFolders = Directory.EnumerateDirectories(fullFolderPath);
            foreach (var subFolderPath in subFolders)
            {
                CopyValidImages(subFolderPath, destinationFolderFullPath);
            }
        }
    }
}