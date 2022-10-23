using CasCap.Models;
using CasCap.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Comparers;
using Polo.Parameters;
using Polo.Parameters.Handler;
using System.Net;

namespace Polo.Commands
{
    public class GooglePhotoCompareCommand : ICommand
    {
        public const string NameLong = "google-photo-compare";
        public const string NameShort = "gpc";
        private readonly ApplicationSettingsReadOnly _applicationSettings;
        private readonly ILogger<GooglePhotoCompareCommand> _logger;

        public GooglePhotoCompareCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger<GooglePhotoCompareCommand> logger)
        {
            _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name => NameLong;

        public string ShortName => NameShort;

        public string Description => "Compares Google Photo album with current folder.";

        public IParameterHandler ParameterHandler => new ParameterHandler
        {
            SourceParameter = new SourceParameter(),
            OutputFolderNameParameter = new OutputFolderNameParameter()
        };

        public void Action(IReadOnlyDictionary<string, string> parameters = null, IEnumerable<ICommand> commands = null)
        {
            var currentFolder = Environment.CurrentDirectory;
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentFolder);
            var outputSubfolderName = ParameterHandler.OutputFolderNameParameter.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var googlePhotoAlbumName = outputSubfolderName;

            // TODO LA - Take secrets from Settings file
            var user = "user_email";
            var clientId = "clientId";
            var clientSecret = "clientSecret";

            var options = new GooglePhotosOptions
            {
                User = user,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scopes = new[] { GooglePhotosScope.Access, GooglePhotosScope.Sharing } //Access+Sharing == full access
            };

            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var client = new HttpClient(handler) { BaseAddress = new Uri(options.BaseAddress) };

            var loggerFactory = LoggerFactory.Create(builder => { });
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

            var album = googlePhotosService.GetAlbumByTitleAsync(googlePhotoAlbumName).GetAwaiter().GetResult();
            _logger.LogInformation($"Album name: {album.title} ID: {album.id} Items count: {album.mediaItemsCount}");

            var albumItems = googlePhotosService.GetMediaItemsByAlbumAsync(album.id).GetAwaiter().GetResult();
            var uploadedFileNames = new List<string>();
            albumItems.ForEach(x => uploadedFileNames.Add(x.filename));
            _logger.LogInformation($"Uploaded files count: {uploadedFileNames.Count}");

            // TODO LA - Check in UTs duplicates
            var imagesLocal = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList() // TODO LA - Move this Select to some extension
                .ForEach(x => imagesLocal.AddRange(Directory.EnumerateFiles(sourceFolderPath, $"*{x}", SearchOption.TopDirectoryOnly)));
            _logger.LogInformation($"Local files count: {imagesLocal.Count}");

            var absentItems = imagesLocal.Except(uploadedFileNames, new FileNameComparer()).ToList();
            _logger.LogInformation($"Absent files count: {absentItems.Count}");

            var index = 0;
            foreach (var absentItem in absentItems)
            {
                _logger.LogInformation($"[{++index}] {absentItem}");
            }
        }
    }
}