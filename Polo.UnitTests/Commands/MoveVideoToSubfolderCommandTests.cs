using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Commands;
using Polo.Options;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class MoveVideoToSubfolderCommandTests : CommandTestBase
    {
        private static readonly IEnumerable<string> videoFileExtensions = new List<string>() { "mkv", "avi", "m2ts", "ts", "mp4", "m4p", "m4v", "mpg", "mpeg" };
        private static readonly IEnumerable<string> _emptyList = Enumerable.Empty<string>();
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings(string.Empty, _emptyList, _emptyList, videoFileExtensions);
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly string VideoSubfolderName = "video";
        private static readonly string AlbumName = "Album1";
        private static readonly Mock<IConsoleService> _consoleServiceMock = new Mock<IConsoleService>();
        private readonly ICommand _sut = new MoveVideoToSubfolderCommand(_mockApplicationOptions, _consoleServiceMock.Object);

        private readonly Folder folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = AlbumName,
                    Files = new List<string>()
                    {
                        "video-1.mp4",
                        "UTP-1.ORF",
                        "UTP-1.jpg"
                    }
                }
            }
        };

        private readonly Folder folderStructureExpected = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = AlbumName,
                    Files = new List<string>()
                    {
                        "UTP-1.ORF",
                        "UTP-1.jpg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = VideoSubfolderName,
                            Files = new List<string>()
                            {
                                "video-1.mp4"
                            }
                        }
                    }
                }
            }
        };

        public MoveVideoToSubfolderCommandTests()
        {
            AddVideoFilesToFolderStructure();
        }

        private void AddVideoFilesToFolderStructure()
        {
            var videoFiles = new List<string>();
            foreach (var extension in videoFileExtensions)
            {
                var fileName = $"video-{extension}.{extension}";
                videoFiles.Add(fileName);
            }

            folderStructureInitial.SubFolders.Where(x => x.Name == AlbumName).ToList().ForEach(x => x.Files.AddRange(videoFiles));
            folderStructureExpected.SubFolders.Where(x => x.Name == AlbumName).ToList()
                .ForEach(x => x.SubFolders.Where(x => x.Name == VideoSubfolderName).ToList()
                    .ForEach(x => x.Files.AddRange(videoFiles)));
        }

        [Fact]
        public void Action_Should_Create_Video_Folder_And_Move_Video_Files_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, AlbumName);

            // Act
            _sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        ~MoveVideoToSubfolderCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
