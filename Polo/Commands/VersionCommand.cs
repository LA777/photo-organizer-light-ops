using Microsoft.Extensions.Logging;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters.Handler;
using Polo.Abstractions.Wrappers;
using System.Reflection;

namespace Polo.Commands;

public class VersionCommand : ICommand
{
    private const string NameLong = "version";
    private const string NameShort = "v";
    private readonly IConsoleWrapper _consoleWrapper;
    private readonly ILogger<VersionCommand> _logger;

    public VersionCommand(IConsoleWrapper consoleWrapper, ILogger<VersionCommand> logger)
    {
        _consoleWrapper = consoleWrapper ?? throw new ArgumentNullException(nameof(consoleWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => NameLong;

    public string ShortName => NameShort;

    public string Description => "Shows application version.";

    public IParameterHandler ParameterHandler { get; } = null!;

    public Task ActionAsync(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(version));
        var versionText = version?.ToString() ?? "Version is not specified.";
        _logger.LogTrace(versionText);
        _consoleWrapper.WriteLine(versionText);

        return Task.CompletedTask;
    }
}