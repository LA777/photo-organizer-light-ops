using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.UnitTests.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Polo.UnitTests.Commands;

[Collection("Sequential")]
public class MoveVideoToSubfolderCommandTests : CommandTestBase
{
    private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings() { VideoFileExtensions = FileExtension.VideoExtensions };
    private static readonly string _albumName = "Album1";
    private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly ICommand _sut = new MoveVideoToSubfolderCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

    private readonly Folder _folderStructureInitial = new Folder()
    {
        SubFolders = new List<Folder>()
        {
            new Folder()
            {
                Name = _albumName,
                Files = new List<PhotoFile>()
                {
                    new PhotoFile("video-1", FileExtension.Mp4),
                    new PhotoFile("UTP-1", FileExtension.Orf),
                    new PhotoFile("UTP-1", FileExtension.Jpg)
                }
            }
        }
    };

    private readonly Folder _folderStructureExpected = new Folder()
    {
        SubFolders = new List<Folder>()
        {
            new Folder()
            {
                Name = _albumName,
                Files = new List<PhotoFile>()
                {
                    new PhotoFile("UTP-1", FileExtension.Orf),
                    new PhotoFile("UTP-1", FileExtension.Jpg)
                },
                SubFolders = new List<Folder>()
                {
                    new Folder()
                    {
                        Name = Constants.VideoFolderName,
                        Files = new List<PhotoFile>()
                        {
                            new PhotoFile("video-1", FileExtension.Mp4)
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
        var videoFiles = FileExtension.VideoExtensions.Select(extension => new PhotoFile($"video-{extension.TrimStart('.')}", extension)).ToList();

        _folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList().ForEach(x => x.Files.AddRange(videoFiles));
        _folderStructureExpected.SubFolders.Where(x => x.Name == _albumName).ToList()
            .ForEach(x => x.SubFolders.Where(folder => folder.Name == Constants.VideoFolderName).ToList()
                .ForEach(folder => folder.Files.AddRange(videoFiles)));
    }

    [Fact]
    public async Task Action_Should_Create_Video_Folder_And_Move_Video_Files_Test_Async()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

        // Act
        await _sut.ActionAsync();

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    ~MoveVideoToSubfolderCommandTests()
    {
        ReleaseUnmanagedResources();
    }
}
