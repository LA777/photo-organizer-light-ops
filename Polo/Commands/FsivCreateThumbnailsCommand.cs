using CasCap.Common.Extensions;
using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.DataProviders;
using Polo.Abstractions.Entities;
using Polo.Abstractions.Enums;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.DataProviders;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands
{
    public class FsivCreateThumbnailsCommand : ICommand
    {
        public const string NameLong = "fsiv-create-thumbnails";
        public const string NameShort = "fct";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger _logger;
        private ISqLiteProvider _sqLiteProvider = null!;

        public FsivCreateThumbnailsCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Creates thumbnails of images and puts them into FastStone Image Viewer database.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            DestinationParameter = new DestinationParameter(),
            RecursiveParameter = new RecursiveParameter(),
            FsivThumbnailSizeParameter = new FsivThumbnailSizeParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);
            var destinationFolderPath = ParameterHandler.DestinationParameter!.Initialize(parameters, null!);
            const string thumbnailDatabaseFileName = "FSIV.db";
            var thumbnailDatabaseFileFullPath = Path.GetFullPath(thumbnailDatabaseFileName, destinationFolderPath);
            var thumbnailSize = ParameterHandler.FsivThumbnailSizeParameter!.Initialize(parameters, _applicationSettings.FsivThumbnailSize);

            if (!File.Exists(thumbnailDatabaseFileFullPath))
            {
                throw new ArgumentException($"Destination folder is not containing {thumbnailDatabaseFileName} file.");
            }

            var isRecursive = ParameterHandler.RecursiveParameter!.Initialize(parameters, true);

            _sqLiteProvider = new SqLiteProvider(thumbnailDatabaseFileFullPath);
            try
            {
                ProcessFiles(sourceFolderPath, isRecursive, thumbnailSize);
            }
            catch (Exception exception)
            {
                // ReSharper disable once StructuredMessageTemplateProblem
                _logger.Information($"ERROR! {exception.Message}");
            }
            finally
            {
                _sqLiteProvider.Dispose();
            }
        }

        private void ProcessFiles(string sourceFolderPath, bool isRecursive, int thumbnailSize)
        {
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            const int epochTimeDifference = 235094400;
            var parentFolderName = $"{sourceFolderPath.ToUpperInvariant()}\\";
            var fsivFolderDb = _sqLiteProvider.GetFsivFolderByFolderName(parentFolderName);
            var isFolderInDb = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (fsivFolderDb == null)
            {
                isFolderInDb = false;
                var nowUnixTime = DateTime.Now.ToUnixTime();
                var fsivUnixTime = (int)nowUnixTime - epochTimeDifference;
                var folder = new FsivFolder
                {
                    FolderName = parentFolderName,
                    LastAccess = fsivUnixTime
                };
                folder.FolderId = _sqLiteProvider.AddFolder(folder);
                fsivFolderDb = folder;
            }

            foreach (var (imageFilePath, index) in imagesForProcess.WithIndex())
            {
                var fileInfo = new FileInfo(imageFilePath);
                if (isFolderInDb)
                {
                    var fsivFileDb = _sqLiteProvider.GetFsivFileByFileNameAndParentFolderId(fileInfo.Name.ToUpperInvariant(), fsivFolderDb.FolderId);

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (fsivFileDb != null)
                    {
                        if (fsivFileDb.ImgSize1 == thumbnailSize || fsivFileDb.ImgSize2 == thumbnailSize)
                        {
                            _logger.Information($"Thumbnail exists already for file: {imageFilePath}");
                            continue;
                        }

                        // image exists but Thumbnail size is different
                        var magickImageInfo1 = new MagickImageInfo(imageFilePath);
                        using var image1 = new MagickImage(imageFilePath);

                        image1.ResizeByLongSide(thumbnailSize, magickImageInfo1);

                        fsivFileDb.ImgSize1 = thumbnailSize;
                        fsivFileDb.Img1 = image1.ToByteArray();

                        var result = _sqLiteProvider.UpdateFile(fsivFileDb);

                        _logger.Information(result ? $"Thumbnail updated for file: {imageFilePath}" : $"Thumbnail was not updated for file: {imageFilePath}");

                        continue;
                    }
                }

                try
                {
                    var magickImageInfo = new MagickImageInfo(imageFilePath);
                    using var image = new MagickImage(imageFilePath);

                    image.ResizeByLongSide(thumbnailSize, magickImageInfo);
                    var fileTime = fileInfo.LastWriteTime.ToUnixTime();
                    var fsivFileTime = (int)fileTime - epochTimeDifference;

                    var fsivFile = new FsivFile
                    {
                        FolderId = fsivFolderDb.FolderId,
                        FileName = fileInfo.Name.ToUpperInvariant(),
                        ItemNo = index,
                        IsFolder = FsivItemType.File,
                        FileTime = fsivFileTime,
                        FileSize = (int)fileInfo.Length,
                        Width = magickImageInfo.Width,
                        Height = magickImageInfo.Height,
                        ImageType = GetFsivImageTypeByExtension(fileInfo.Extension),
                        ImgSize1 = thumbnailSize,
                        Img1 = image.ToByteArray()
                    };

                    _sqLiteProvider.AddFile(fsivFile);
                    _logger.Information($"Thumbnail added for file: {imageFilePath}");
                }
                catch (Exception exception)
                {
                    _logger.Information($"ERROR! {exception.Message}");
                }
            }

            if (!isRecursive)
            {
                return;
            }

            var subFolders = Directory.EnumerateDirectories(sourceFolderPath);
            foreach (var subFolderPath in subFolders)
            {
                ProcessFiles(subFolderPath, isRecursive, thumbnailSize);
            }
        }

        private static FsivImageType GetFsivImageTypeByExtension(string fileExtensionWithDot)
        {
            switch (fileExtensionWithDot.ToUpperInvariant())
            {
                case ".JPG":
                case ".JPEG":
                    return FsivImageType.Jpg;
                case ".BMP":
                    return FsivImageType.Bmp;
                case ".GIF":
                    return FsivImageType.Gif;
                case ".Png":
                    return FsivImageType.Png;
                case ".WEBP":
                    return FsivImageType.Webp;
                case ".MP4":
                    return FsivImageType.Mp4;
                default:
                    throw new ArgumentException($"Unknown extension: {fileExtensionWithDot}");
            }
        }
    }
}