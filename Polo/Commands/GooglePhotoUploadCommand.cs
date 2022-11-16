using CasCap.Models;
using CasCap.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using System.Net;

namespace Polo.Commands
{
    public class GooglePhotoUploadCommand : ICommand
    {
        public const string NameLong = "google-photo-upload";
        public const string NameShort = "gpu";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger<GooglePhotoUploadCommand> _logger;

        public GooglePhotoUploadCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger<GooglePhotoUploadCommand> logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Uploads to the Google Photo.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
        {
            var currentFolder = Environment.CurrentDirectory;
            var directoryInfo = new DirectoryInfo(currentFolder);
            var folderName = directoryInfo.Name;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentFolder);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter!.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var destinationFolder = Path.GetFullPath(outputFolderName, sourceFolderPath);

            // TODO LA - Move secrets to Settings file
            var user = "user";
            var clientId = "clientId";
            var clientSecret = "secret";

            var options = new GooglePhotosOptions
            {
                User = user,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scopes = new[] { GooglePhotosScope.Access, GooglePhotosScope.Sharing } //Access+Sharing == full access
            };

            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var client = new HttpClient(handler) { BaseAddress = new Uri(options.BaseAddress) };


            var loggerFactory = LoggerFactory.Create(_ => { });
            var logger = loggerFactory.CreateLogger<GooglePhotosService>();

            // TODO LA - Move to DI container
            var googlePhotosService = new GooglePhotosService(logger, Options.Create(options), client);
            var isLoggedIn = googlePhotosService.LoginAsync().GetAwaiter().GetResult();
            if (!isLoggedIn)
            {
                throw new UnauthorizedAccessException("Unauthorized access");
            }

            _logger.LogInformation("Login successful");

            // TODO LA - Show and log User/Account information

            var album = googlePhotosService.GetOrCreateAlbumAsync(folderName).GetAwaiter().GetResult();

            _logger.LogInformation($"Album created: {album!.title} ID: {album.id} Items count: {album.mediaItemsCount}");
            // TODO LA - Check in UTs duplicates
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList() // TODO LA - Move this Select to some extension
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(destinationFolder, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            foreach (var imageForProcess in imagesForProcess)
            {
                _logger.LogInformation($"Uploading photo: {Path.GetFileName(imageForProcess)}");
                var newMediaItemResult = googlePhotosService.UploadSingle(imageForProcess, album.id, null, GooglePhotosUploadMethod.ResumableSingle).GetAwaiter().GetResult();
                if (newMediaItemResult is null)
                {
                    throw new Exception("media item upload failed!");
                }

                _logger.LogInformation($"Photo uploaded: {newMediaItemResult.mediaItem.filename} ID: {newMediaItemResult.mediaItem.id}");
            }

            var albumMediaItems = googlePhotosService.GetMediaItemsByAlbumAsync(album.id).GetAwaiter().GetResult();
            if (albumMediaItems is null)
            {
                throw new Exception("retrieve media items by album id failed!");
            }

            _logger.LogInformation($"Album photos count: {albumMediaItems.Count}");
            _logger.LogInformation($"Contributor: {albumMediaItems.FirstOrDefault()?.contributorInfo?.displayName}");
        }
    }
}