using Polo.UnitTests.FileUtils;
using System;

namespace Polo.UnitTests.Commands
{
    public abstract class CommandTestBase : IDisposable
    {
        internal static void ReleaseUnmanagedResources()
        {
            FileHelper.TryDeleteTestFolder();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~CommandTestBase()
        {
            ReleaseUnmanagedResources();
        }
    }
}
