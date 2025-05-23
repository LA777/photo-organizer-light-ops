using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Options;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Settings;
using System;

namespace Polo.UnitTests.Commands;

public abstract class CommandTestBase : BaseTest, IDisposable
{
    protected readonly FileHelper FileHelper;

    protected CommandTestBase()
    {
        // Build configuration
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", optional: false)
            .AddJsonFile("testsettings.local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Retrieve settings from configuration
        var testSettings = new TestSettings();
        config.GetSection("TestSettings").Bind(testSettings);

        FileHelper = new FileHelper(testSettings.TestFolderPath);
        FileHelper.TryDeleteTestFolder();
    }

    internal void ReleaseUnmanagedResources()
    {
        FileHelper.TryDeleteTestFolder();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    internal static IOptions<ApplicationSettingsReadOnly> GetOptions(ApplicationSettings applicationSettings)
    {
        var validApplicationSettingsReadOnly = new ApplicationSettingsReadOnly(applicationSettings);
        var mockApplicationOptions = Options.Create(validApplicationSettingsReadOnly);

        return mockApplicationOptions;
    }

    ~CommandTestBase()
    {
        ReleaseUnmanagedResources();
    }
}
