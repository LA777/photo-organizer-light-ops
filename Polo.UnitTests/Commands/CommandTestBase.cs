using Microsoft.Extensions.Options;
using Polo.Abstractions.Options;
using Polo.UnitTests.FileUtils;
using System;

namespace Polo.UnitTests.Commands
{
    public abstract class CommandTestBase : IDisposable
    {
        protected CommandTestBase()
        {
            FileHelper.TryDeleteTestFolder();
        }

        internal static void ReleaseUnmanagedResources()
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
}
