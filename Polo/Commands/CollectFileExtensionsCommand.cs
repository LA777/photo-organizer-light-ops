using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;

namespace Polo.Commands;

public class CollectFileExtensionsCommand : ICommand
{
    private const string NameLong = "collect-file-extentions";
    private const string NameShort = "cfe";
    private readonly ILogger<CollectFileExtensionsCommand> _logger;
    private readonly ApplicationSettingsReadOnly _applicationSettings;

    public CollectFileExtensionsCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger<CollectFileExtensionsCommand> logger)
    {
        _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => NameLong;

    public string ShortName => NameShort;

    public string Description => "Collect file extensions and writes them to the output file.";

    public IParameterHandler ParameterHandler => new ParameterHandler
    {
        SourceParameter = new SourceParameter(),
        OutputFileNameParameter = new OutputFileNameParameter(),
        RecursiveParameter = new RecursiveParameter()
    };

    public async Task ActionAsync(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
    {
        // TODO LA - Cover with UTs
        var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
        var outputFileName = ParameterHandler.OutputFileNameParameter.Initialize(parameters, _applicationSettings.OutputFileName);
        var isRecursive = ParameterHandler.RecursiveParameter.Initialize(parameters, true);
        var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        HashSet<string> uniqueExtensions = new(StringComparer.OrdinalIgnoreCase);

        _logger.LogTrace("Starting to scan folder: {FolderPath}", sourceFolder);

        try
        {
            foreach (var filePath in Directory.EnumerateFiles(sourceFolder, "*.*", searchOption))
            {
                string extension = Path.GetExtension(filePath);

                // Path.GetExtension returns an empty string if there's no extension (e.g., "myfile")
                // Only add if there's a non-empty extension
                if (!string.IsNullOrEmpty(extension))
                {
                    uniqueExtensions.Add($"'{extension}'");
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Access Error: Could not access some files or directories in '{FolderPath}'. {ErrorMessage}", sourceFolder, ex.Message);
        }
        catch (PathTooLongException ex)
        {
            _logger.LogError(ex, "Path Error: One or more paths were too long in '{FolderPath}'. {ErrorMessage}", sourceFolder, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing '{FolderPath}'. {ErrorMessage}", sourceFolder, ex.Message);
        }

        if (uniqueExtensions.Count > 0)
        {
            var outputFileFullPath = Path.Combine(sourceFolder, outputFileName);

            var sortedExtensions = uniqueExtensions.OrderBy(ext => ext, StringComparer.OrdinalIgnoreCase).ToList();
            var extensionString = string.Join(',', sortedExtensions);

            await File.AppendAllLinesAsync(outputFileFullPath, [extensionString]);

            _logger.LogInformation("Output file path: {OutputFileFullPath}", outputFileFullPath);
        }
        else
        {
            _logger.LogWarning("No extensions found.");
        }
    }
}