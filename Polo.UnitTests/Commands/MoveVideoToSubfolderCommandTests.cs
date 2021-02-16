using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Commands;
using Polo.Options;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using Serilog;
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
        private static readonly IEnumerable<string> _videoFileExtensions = new List<string>() { "mkv", "avi", "m2ts", "ts", "mp4", "m4p", "m4v", "mpg", "mpeg" };
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings(videoFileExtensions: _videoFileExtensions);
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly string _videoSubfolderName = "video";
        private static readonly string _albumName = "Album1";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new MoveVideoToSubfolderCommand(_mockApplicationOptions, _loggerMock.Object);

        private readonly Folder folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("video-1", "mp4"),
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-1", "jpg")
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
                    Name = _albumName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-1", "jpg")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _videoSubfolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("video-1", "mp4")
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
            var videoFiles = new List<FotoFile>();
            foreach (var extension in _videoFileExtensions)
            {
                var file = new FotoFile($"video-{extension}", extension);
                videoFiles.Add(file);
            }

            folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList().ForEach(x => x.Files.AddRange(videoFiles));
            folderStructureExpected.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x => x.SubFolders.Where(x => x.Name == _videoSubfolderName).ToList()
                    .ForEach(x => x.Files.AddRange(videoFiles)));
        }

        [Fact]
        public void Action_Should_Create_Video_Folder_And_Move_Video_Files_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

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
