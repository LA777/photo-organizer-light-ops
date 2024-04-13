using FluentAssertions;
using Polo.UnitTests.Commands;
using System.IO;
using Xunit;
using Polo.UnitTests.Models;
using Polo.Extensions;

namespace Polo.UnitTests.Extensions
{
    [Collection("Sequential")]
    public class FileInfoExtensionTests : CommandTestBase
    {
        private const string FileName = "UTP-1";

        private readonly Folder _folderStructureInitial = new()
        {
            SubFolders =
            [
                new Folder
                {
                    Name = "source-hash",
                    TextFiles = [new(FileName, FileExtension.Txt, "ABCDEFG123456789")]
                }
            ]
        };

        [Fact]
        public void GetFileSha256_Should_Get_File_Hash_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var fileFullPath = Path.Combine(testFolderFullPath, "source-hash", $"{FileName}{FileExtension.Txt}");
            var expectedHashSha256 = "1a6d16d13d1af3c7c8c9dcff85712ba1efb5a2d2e9bf07dd4188349c9793233d7dcc98cd1df053dc811a98638be3d80d5f866bb32a74e60eaaf6027a229facca";
            var fileInfo = new FileInfo(fileFullPath);

            // Act
            var result = fileInfo.GetFileHashSha256();

            // Assert
            result.Should().Be(expectedHashSha256);
        }



        ~FileInfoExtensionTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
