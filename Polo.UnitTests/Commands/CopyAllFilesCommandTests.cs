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
    public class CopyAllFilesCommandTests : CommandTestBase
    {
        private const string DefaultSourceFolderPath = "c:\\";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings() { DefaultSourceFolderPath = DefaultSourceFolderPath };
        private static readonly string _sourceFolderName = "Source Fotos";
        private static readonly string _destinationFolderName = "Destination Fotos";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new CopyAllFilesCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

        private readonly Folder _folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _sourceFolderName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-2", "ORF"),
                        new FotoFile("UTP-3", "ORF"),
                        new FotoFile("UTP-4", "ORF"),
                        new FotoFile("UTP-5", "ORF"),
                        new FotoFile("UTP-6", "ORF"),
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "jpg"),
                        new FotoFile("UTP-3", "jpg"),
                        new FotoFile("UTP-4", "jpg"),
                        new FotoFile("UTP-5", "jpg"),
                        new FotoFile("UTP-6", "jpg")
                    }
                },
                new Folder()
                {
                    Name = _destinationFolderName
                }
            }
        };

        private readonly Folder _folderStructureExpected = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _sourceFolderName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-2", "ORF"),
                        new FotoFile("UTP-3", "ORF"),
                        new FotoFile("UTP-4", "ORF"),
                        new FotoFile("UTP-5", "ORF"),
                        new FotoFile("UTP-6", "ORF"),
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "jpg"),
                        new FotoFile("UTP-3", "jpg"),
                        new FotoFile("UTP-4", "jpg"),
                        new FotoFile("UTP-5", "jpg"),
                        new FotoFile("UTP-6", "jpg")
                    }
                },
                new Folder()
                {
                    Name = _destinationFolderName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-2", "ORF"),
                        new FotoFile("UTP-3", "ORF"),
                        new FotoFile("UTP-4", "ORF"),
                        new FotoFile("UTP-5", "ORF"),
                        new FotoFile("UTP-6", "ORF"),
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "jpg"),
                        new FotoFile("UTP-3", "jpg"),
                        new FotoFile("UTP-4", "jpg"),
                        new FotoFile("UTP-5", "jpg"),
                        new FotoFile("UTP-6", "jpg")
                    }
                }
            }
        };

        [Fact]
        public void Action_Should_Copy_Files_With_Valid_Input_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { SourceParameter.Name, sourceFolderPath },
                { DestinationParameter.Name, destinationFolderPath }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Copy_Files_With_Valid_SourceFolder_Input_Parameter_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { SourceParameter.Name, sourceFolderPath }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Copy_Files_Without_Input_Parameters_And_Valid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>();

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = sourceFolderPath
            };
            var sut = new CopyAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        ~CopyAllFilesCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
