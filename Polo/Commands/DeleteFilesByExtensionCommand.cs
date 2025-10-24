using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;

namespace Polo.Commands;

public class DeleteFilesByExtensionCommand : ICommand
{
    private const string NameLong = "delete-files-by-extension";
    private const string NameShort = "dfbe";
    private readonly ILogger<DeleteFilesByExtensionCommand> _logger;

    public DeleteFilesByExtensionCommand(ILogger<DeleteFilesByExtensionCommand> logger)
    {
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

    public Task ActionAsync(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
    {
        var sourceFolder = ParameterHandler.SourceParameter.Initialize(parameters, Environment.CurrentDirectory);
        var extension = ParameterHandler.ExtensionParameter.Initialize(parameters, null!);
        var isRecursive = ParameterHandler.RecursiveParameter.Initialize(parameters, true);
        var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var allFiles = Directory.GetFiles(sourceFolder, $"*.{extension}", searchOption);
        var filesCount = allFiles.Count();

        foreach (var (fileFullPath, index) in allFiles.WithIndex())
        {
            File.Delete(fileFullPath);
            _logger.LogInformation($"[{index}/{filesCount}] File was deleted: {fileFullPath}");
        }

        return Task.CompletedTask;
    }
}