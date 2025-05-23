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
public class RawCommandTests : CommandTestBase
{
    private static readonly string _albumName = "Album1";
    private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { FileExtension.Jpg, FileExtension.Jpeg };
    private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
    {
        FileForProcessExtensions = FileExtension.JpegExtensions,
        RawFileExtensions = FileExtension.RawExtensions,
        RawFolderName = Constants.RawFolderName
    };
    private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly ICommand _sut = new RawCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);


    private readonly Folder _folderStructureInitial = new Folder()
    {
        SubFolders = new List<Folder>()
        {
            new Folder()
            {
                Name = _albumName,
                Files = new List<PhotoFile>()
                {
                    new PhotoFile("UTP-1", FileExtension.Orf),
                    new PhotoFile("UTP-2", FileExtension.Orf),
                    new PhotoFile("UTP-3", FileExtension.Orf),
                    new PhotoFile("UTP-1", FileExtension.Jpg),
                    new PhotoFile("UTP-2", FileExtension.Jpeg.ToUpper()),
                    new PhotoFile("UTP-3", FileExtension.Jpg.ToUpper())
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
                    new PhotoFile("UTP-1", FileExtension.Jpg),
                    new PhotoFile("UTP-2", FileExtension.Jpeg.ToUpper()),
                    new PhotoFile("UTP-3", FileExtension.Jpg.ToUpper())
                },
                SubFolders = new List<Folder>()
                {
                    new Folder()
                    {
                        Name = Constants.RawFolderName,
                        Files = new List<PhotoFile>()
                        {
                            new PhotoFile("UTP-1", FileExtension.Orf),
                            new PhotoFile("UTP-2", FileExtension.Orf),
                            new PhotoFile("UTP-3", FileExtension.Orf)
                        }
                    }

                }
            }
        }
    };

    public RawCommandTests()
    {
        AddRawFilesToStructure();
    }

    private void AddRawFilesToStructure()
    {
        var rawFiles = new List<PhotoFile>();
        var jpgFiles = new List<PhotoFile>();

        foreach (var extension in FileExtension.RawExtensions)
        {
            var rawFile = new PhotoFile($"image-{extension.TrimStart('.')}", extension);
            var jpgFile = new PhotoFile($"image-{extension.TrimStart('.')}", FileExtension.Jpg);
            rawFiles.Add(rawFile);
            jpgFiles.Add(jpgFile);
        }

        _folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList()
            .ForEach(x =>
            {
                x.Files.AddRange(rawFiles);
                x.Files.AddRange(jpgFiles);
            });

        _folderStructureExpected.SubFolders.Where(x => x.Name == _albumName).ToList()
            .ForEach(x =>
            {
                x.Files.AddRange(jpgFiles);
                x.SubFolders.Where(folder => folder.Name == Constants.RawFolderName).ToList().ForEach(folder => folder.Files.AddRange(rawFiles));
            });
    }

    [Fact]
    public async Task Action_Should_Create_Raw_Folder_And_Move_Raw_Files_Test()
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

    [Fact]
    public async Task Action_Should_Create_Raw_Folder_And_Move_Raw_Files_If_Setting_Have_Duplicate_JpegFileExtension_Test()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

        var jpegFileExtensionsWithDuplicates = new List<string>() { FileExtension.Jpeg, FileExtension.Jpg, FileExtension.Jpeg, FileExtension.Jpg };
        var applicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = jpegFileExtensionsWithDuplicates,
            RawFileExtensions = FileExtension.RawExtensions,
            RawFolderName = Constants.RawFolderName
        };
        var sut = new RawCommand(GetOptions(applicationSettings), _loggerMock.Object);

        // Act
        await sut.ActionAsync();

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    [Fact]
    public async Task Action_Should_Create_Raw_Folder_And_Move_Raw_Files_If_Setting_Have_Duplicate_RawFileExtension_Test()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

        var rawFileExtensionsWithDuplicates = new List<string>();
        rawFileExtensionsWithDuplicates.AddRange(FileExtension.RawExtensions);
        rawFileExtensionsWithDuplicates.AddRange(FileExtension.RawExtensions);
        var applicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = _jpegFileExtensions,
            RawFileExtensions = rawFileExtensionsWithDuplicates,
            RawFolderName = Constants.RawFolderName
        };
        var sut = new RawCommand(GetOptions(applicationSettings), _loggerMock.Object);

        // Act
        await sut.ActionAsync();

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    ~RawCommandTests()
    {
        ReleaseUnmanagedResources();
    }
}
