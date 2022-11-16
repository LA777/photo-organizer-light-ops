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
    public class MoveCorruptedImagesCommand : ICommand
    {
        public const string NameLong = "move-corrupted-images";
        public const string NameShort = "mci";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;

        public MoveCorruptedImagesCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Move corrupted (not all pixels are present) images into destination folder.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter(),
            RecursiveParameter = new RecursiveParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);
            var destinationFolderPath = ParameterHandler.DestinationParameter!.Initialize(parameters, string.Empty);
            var isRecursive = ParameterHandler.RecursiveParameter!.Initialize(parameters, true);

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            MoveCorruptedImages(sourceFolderPath, destinationFolderPath, isRecursive);
        }

        private void MoveCorruptedImages(string fullFolderPath, string destinationFolderFullPath, bool isRecursive)
        {
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var imageFiles = new List<string>();
            _applicationSettings.ImageFileExtensions.Distinct().ToList()
                .ForEach(x => imageFiles.AddRange(Directory.EnumerateFiles(fullFolderPath, $"*{x}", searchOption)));

            var count = imageFiles.Count;

            foreach (var (imageFilePath, index) in imageFiles.WithIndex())
            {
                var imageFileInfo = new FileInfo(imageFilePath);
                var destinationImagePath = imageFileInfo.GenerateFileFullPath(destinationFolderFullPath);

                try
                {
                    using var image = new MagickImage(imageFilePath);
                    var pixels = image.GetPixels();
                    var lastPixel = pixels.Last();
                    var color = lastPixel.ToColor();
                    const ushort grey = 32896;

                    if (color!.R != grey || color.G != grey || color.B != grey)
                    {
                        _logger.Information($"[{index}/{count}] File skipped: {destinationImagePath}");
                        continue;
                    }

                    File.Move(imageFilePath, destinationImagePath);
                    _logger.Information($"[{index}/{count}] Corrupted file copied: {destinationImagePath}");
                }
                catch (Exception)
                {
                    File.Move(imageFilePath, destinationImagePath);
                    _logger.Information($"[{index}/{count}] Invalid file copied: {imageFilePath}");
                }
            }
        }
    }
}