using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System.Text.RegularExpressions;

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

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);
            var destinationFolderPath = ParameterHandler.DestinationParameter.Initialize(parameters, string.Empty);

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
                    var destinationImagePath = GenerateFileFullPath(imageFileInfo, destinationFolderFullPath);

                    File.Copy(imageFilePath, destinationImagePath);
                    _logger.Information($"Valid file copied: {destinationImagePath}");
                }
                catch (Exception e)
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

        private static string GenerateFileFullPath(FileInfo fileInfo, string destinationFolderFullPath)
        {
            var destinationImagePath = Path.Combine(destinationFolderFullPath, fileInfo.Name);
            if (!File.Exists(destinationImagePath))
            {
                return destinationImagePath;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);

            const string regexPattern = "\\([0-9]+\\)$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(fileNameWithoutExtension))
            {
                destinationImagePath = Path.Combine(destinationFolderFullPath, $"{fileNameWithoutExtension} (0){fileInfo.Extension}");

                if (!File.Exists(destinationImagePath))
                {
                    return destinationImagePath;
                }
            }

            string nameWithoutIndex;
            int index;

            do
            {
                const string regexPatternForIndex = "[0-9]+";
                var regexIndex = new Regex(regexPatternForIndex, RegexOptions.IgnoreCase);

                var destinationFileNameWithoutExtension = Path.GetFileNameWithoutExtension(destinationImagePath); // '101 (0)'
                var indexInBraces = regex.Match(destinationFileNameWithoutExtension); // '(0)'

                var match = regexIndex.Match(indexInBraces.Value);
                var value = match.Value; // '0'
                index = Convert.ToInt32(value); // 0
                index++;

                var length = indexInBraces.Length;
                nameWithoutIndex = destinationFileNameWithoutExtension.Substring(0, destinationFileNameWithoutExtension.Length - length); // '101'

                destinationImagePath = Path.Combine(destinationFolderFullPath, $"{nameWithoutIndex}({index}){fileInfo.Extension}");
            } while (File.Exists(destinationImagePath));

            return destinationImagePath;
        }
    }
}