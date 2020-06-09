using FluentAssertions;
using Polo.Commands;
using Polo.UnitTests.FileUtils;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Polo.UnitTests.Commands
{
    public class RawCommandTests : IDisposable
    {
        private readonly ICommand _sut = new RawCommand();

        [Fact]
        public void Action_Should_Create_Raw_Folder_And_Move_Raw_Files_Test()
        {
            // Arrange
            var testFolderPath = FileHelper.CreateTestFolder();
            FileHelper.CreateJpegFiles(testFolderPath);
            FileHelper.CreateRawFiles(testFolderPath);
            Environment.CurrentDirectory = testFolderPath;

            // Act
            _sut.Action();

            // Assert
            var allFiles = Directory.EnumerateFiles(testFolderPath, "*.*", SearchOption.AllDirectories);
            allFiles.Count().Should().Be(FileHelper.FileLimit * 2);
            var rawFolderPath = Path.Join(testFolderPath, "RAW");
            Directory.Exists(rawFolderPath).Should().BeTrue();

            var jpegFiles = Directory.EnumerateFiles(testFolderPath, $"*.{FileHelper.JpegExtension}", SearchOption.TopDirectoryOnly);
            jpegFiles.Count().Should().Be(FileHelper.FileLimit);

            var rawFiles = Directory.EnumerateFiles(rawFolderPath, $"*.{FileHelper.RawExtension}", SearchOption.TopDirectoryOnly);
            rawFiles.Count().Should().Be(FileHelper.FileLimit);
        }

        private static void ReleaseUnmanagedResources()
        {
            FileHelper.TryDeleteTestFolder();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~RawCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
