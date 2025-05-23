using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.Parameters;
using Polo.UnitTests.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Polo.UnitTests.Commands;

[Collection("Sequential")]
public class MoveAllFilesCommandTests : CommandTestBase
{
    private const string DefaultSourceFolderPath = "d:\\";
    private static readonly ApplicationSettings _validApplicationSettings = new() { DefaultSourceFolderPath = DefaultSourceFolderPath };
    private static readonly Mock<ILogger> _loggerMock = new();

    private readonly Folder _folderStructureExpected = new()
    {
        SubFolders = new List<Folder>
        {
            new()
            {
                Name = Constants.SourceFolderName,
                Files = new List<PhotoFile>()
            },
            new()
            {
                Name = Constants.DestinationFolderName,
                Files = new List<PhotoFile>
                {
                    new("UTP-1", FileExtension.Orf),
                    new("UTP-2", FileExtension.Orf),
                    new("UTP-3", FileExtension.Orf),
                    new("UTP-4", FileExtension.Orf),
                    new("UTP-5", FileExtension.Orf),
                    new("UTP-6", FileExtension.Orf),
                    new("UTP-1", FileExtension.Jpg),
                    new("UTP-2", FileExtension.Jpg),
                    new("UTP-3", FileExtension.Jpg),
                    new("UTP-4", FileExtension.Jpg),
                    new("UTP-5", FileExtension.Jpg),
                    new("UTP-6", FileExtension.Jpg)
                }
            }
        }
    };

    private readonly Folder _folderStructureInitial = new()
    {
        SubFolders = new List<Folder>
        {
            new()
            {
                Name = Constants.SourceFolderName,
                Files = new List<PhotoFile>
                {
                    new("UTP-1", FileExtension.Orf),
                    new("UTP-2", FileExtension.Orf),
                    new("UTP-3", FileExtension.Orf),
                    new("UTP-4", FileExtension.Orf),
                    new("UTP-5", FileExtension.Orf),
                    new("UTP-6", FileExtension.Orf),
                    new("UTP-1", FileExtension.Jpg),
                    new("UTP-2", FileExtension.Jpg),
                    new("UTP-3", FileExtension.Jpg),
                    new("UTP-4", FileExtension.Jpg),
                    new("UTP-5", FileExtension.Jpg),
                    new("UTP-6", FileExtension.Jpg)
                }
            },
            new()
            {
                Name = Constants.DestinationFolderName
            }
        }
    };

    private readonly ICommand _sut = new MoveAllFilesCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

    [Fact]
    public async Task Action_Should_Move_Files_With_Valid_Input_Parameters_Test_Async()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        var sourceFolderPath = Path.Combine(testFolderFullPath, Constants.SourceFolderName);
        var destinationFolderPath = Path.Combine(testFolderFullPath, Constants.DestinationFolderName);
        Environment.CurrentDirectory = testFolderFullPath;
        var parameters = new Dictionary<string, string>
        {
            { new SourceParameter().Name, sourceFolderPath },
            { new DestinationParameter().Name, destinationFolderPath }
        };

        // Act
        await _sut.ActionAsync(parameters);

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    [Fact]
    public async Task Action_Should_Move_Files_With_Valid_SourceFolder_Input_Parameter_Test_Async()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        var sourceFolderPath = Path.Combine(testFolderFullPath, Constants.SourceFolderName);
        var destinationFolderPath = Path.Combine(testFolderFullPath, Constants.DestinationFolderName);
        Environment.CurrentDirectory = destinationFolderPath;
        var parameters = new Dictionary<string, string>
        {
            { new SourceParameter().Name, sourceFolderPath }
        };

        // Act
        await _sut.ActionAsync(parameters);

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    [Fact]
    public async Task Action_Should_Move_Files_Without_Parameters_And_Valid_SourceFolder_Settings_Test_Async()
    {
        // Arrange
        var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
        var destinationFolderPath = Path.Combine(testFolderFullPath, Constants.DestinationFolderName);
        Environment.CurrentDirectory = destinationFolderPath;
        var parameters = new Dictionary<string, string>();

        // TODO LA - Restore this function

        // Act
        await _sut.ActionAsync(parameters);

        // Assert
        var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
        folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
    }

    ~MoveAllFilesCommandTests()
    {
        ReleaseUnmanagedResources();
    }
}