using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.Parameters;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class MoveAllFilesCommandTests : CommandTestBase
    {
        private const string DefaultSourceFolderPath = "c:\\";
        private static readonly ApplicationSettings _validApplicationSettings = new() { DefaultSourceFolderPath = DefaultSourceFolderPath };
        private static readonly Mock<ILogger> _loggerMock = new();

        private readonly Folder _folderStructureExpected = new()
        {
            SubFolders = new List<Folder>
            {
                new()
                {
                    Name = Constants.SourceFolderName,
                    Files = new List<FotoFile>()
                },
                new()
                {
                    Name = Constants.DestinationFolderName,
                    Files = new List<FotoFile>
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
                    Files = new List<FotoFile>
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
        public void Action_Should_Move_Files_With_Valid_Input_Parameters_Test()
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
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Files_With_Valid_SourceFolder_Input_Parameter_Test()
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
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Files_Without_Parameters_And_Valid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, Constants.SourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, Constants.DestinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>();

            var validApplicationSettings = new ApplicationSettings
            {
                DefaultSourceFolderPath = sourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        ~MoveAllFilesCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}