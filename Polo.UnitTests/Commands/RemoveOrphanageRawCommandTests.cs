using FluentAssertions;
using Polo.Commands;
using Polo.UnitTests.FileUtils;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Polo.UnitTests.Commands
{
    public class RemoveOrphanageRawCommandTests : IDisposable
    {
        private readonly ICommand _sut = new RemoveOrphanageRawCommand();

        [Fact]
        public void Action_Should_Create_Raw_Folder_And_Move_Raw_Files_Test()
        {
            // Arrange
            var testFolderPath = FileHelper.CreateTestFolder();
            FileHelper.CreateJpegFiles(testFolderPath);
            var rawFolderPath = FileHelper.CreateRawFolder();
            FileHelper.CreateRawFiles(rawFolderPath);
            Environment.CurrentDirectory = testFolderPath;
            const int jpegFilesDeleteCount = 2;
            FileHelper.DeleteRandomJpegFiles(testFolderPath, jpegFilesDeleteCount);
            const int expectedFilesCount = FileHelper.FileLimit - jpegFilesDeleteCount;

            // Act
            _sut.Action();

            // Assert
            var jpegFiles = Directory.EnumerateFiles(testFolderPath, $"*.{FileHelper.JpegExtension}", SearchOption.TopDirectoryOnly).ToList();
            jpegFiles.Count().Should().Be(expectedFilesCount);

            var rawFiles = Directory.EnumerateFiles(rawFolderPath, $"*.{FileHelper.RawExtension}", SearchOption.TopDirectoryOnly).ToList();
            rawFiles.Count().Should().Be(expectedFilesCount);

            var jpegFilesNames = jpegFiles.Select(Path.GetFileNameWithoutExtension);
            var rawFilesNames = rawFiles.Select(Path.GetFileNameWithoutExtension);
            rawFilesNames.Should().BeEquivalentTo(jpegFilesNames);
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

        ~RemoveOrphanageRawCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
