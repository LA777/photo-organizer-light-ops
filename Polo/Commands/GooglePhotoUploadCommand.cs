using CasCap.Models;
using CasCap.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Polo.Commands
{
    public class GooglePhotoUploadCommand : ICommand
    {
        private readonly ILogger<GooglePhotoUploadCommand> _logger;
        private readonly ApplicationSettingsReadOnly _applicationSettings;

        public readonly ParameterHandler ParameterHandler = new ParameterHandler()
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter()
        };

        public string Name => "google-photo-upload";

        public string ShortName => "gpu";

        public string Description => "Uploads to the Google Photo.";

        public GooglePhotoUploadCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger<GooglePhotoUploadCommand> logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentFolder = Environment.CurrentDirectory;
            var directoryInfo = new DirectoryInfo(currentFolder);
            var folderName = directoryInfo.Name;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentFolder);
            var outputSubfolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var outputDirectory = Path.Combine(sourceFolderPath, outputSubfolderName);

            // TODO LA - Move secrets to Settings file
            var user = "user";
            var clientId = "clientId";
            var clientSecret = "secret";

            var options = new GooglePhotosOptions
            {
                User = user,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scopes = new[] { GooglePhotosScope.Access, GooglePhotosScope.Sharing },//Access+Sharing == full access
            };

            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var client = new HttpClient(handler) { BaseAddress = new Uri(options.BaseAddress) };


            var loggerFactory = LoggerFactory.Create(builder =>
            {

            });
            var logger = loggerFactory.CreateLogger<GooglePhotosService>();

            // TODO LA - Move to DI container
            var googlePhotosService = new GooglePhotosService(logger, Options.Create(options), client);
            var isLogined = googlePhotosService.LoginAsync().GetAwaiter().GetResult();
            if (!isLogined)
            {
                throw new UnauthorizedAccessException("Unauthorized access");
            }

            _logger.LogInformation("Login successful");

            // TODO LA - Show and log User/Account information

            var album = googlePhotosService.GetOrCreateAlbumAsync(folderName).GetAwaiter().GetResult();

            _logger.LogInformation($"Album created: {album.title} ID: {album.id} Items count: {album.mediaItemsCount}");
            // TODO LA - Check in UTs duplicates
            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()// TODO LA - Move this Select to some extension
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(outputDirectory, $"*.{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            foreach (var imageForProcess in imagesForProcess)
            {
                _logger.LogInformation($"Uploading photo: {Path.GetFileName(imageForProcess)}");
                var newMediaItemResult = googlePhotosService.UploadSingle(imageForProcess, album.id, null, GooglePhotosUploadMethod.ResumableSingle).GetAwaiter().GetResult();
                if (newMediaItemResult is null) throw new Exception("media item upload failed!");
                _logger.LogInformation($"Photo uploaded: {newMediaItemResult.mediaItem.filename} ID: {newMediaItemResult.mediaItem.id}");
            }

            var albumMediaItems = googlePhotosService.GetMediaItemsByAlbumAsync(album.id).GetAwaiter().GetResult();
            if (albumMediaItems is null) throw new Exception("retrieve media items by album id failed!");
            _logger.LogInformation($"Album photos count: {albumMediaItems.Count}");
            _logger.LogInformation($"Contributor: {albumMediaItems.FirstOrDefault()?.contributorInfo?.displayName}");
        }
    }
}
