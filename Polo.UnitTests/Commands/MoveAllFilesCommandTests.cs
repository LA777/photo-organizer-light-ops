using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
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
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings() { DefaultSourceFolderPath = DefaultSourceFolderPath };
        private static readonly string _sourceFolderName = "Source Fotos";
        private static readonly string _destinationFolderName = "Destination Fotos";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new MoveAllFilesCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

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
        public void Action_Should_Move_Files_With_Two_Valid_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath },
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Valid_DestinationFolder_And_Invalid_SourceFolder_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath },
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{sourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_DestinationFolder_And_Valid_SourceFolder_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath },
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{destinationFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Move_Files_With_Valid_SourceFolder_Parameter_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_SourceFolder_Parameter_And_Valid_SourceFolder_Setting_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{sourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Move_Files_Without_Parameters_And_Valid_SourceFolder_Settings_Test()
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
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Files_If_Parameters_Null_And_Valid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = sourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Files_With_Valid_DestinationFolder_Parameter_And_Valid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            var validApplicationSettings = new ApplicationSettings()
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

        [Fact]
        public void Action_Should_Move_Files_With_Valid_SourceFolder_Parameter_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var invalidSourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, sourceFolderPath }
            };

            var invalidApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = invalidSourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(invalidApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_SourceFolder_Parameter_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>();

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = sourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{sourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_Parameters_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var invalidSourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var invalidDestinationFolderPath = Path.Combine(testFolderFullPath, "no such folder 2");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;

            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, invalidSourceFolderPath },
                { MoveAllFilesCommand.DestinationFolderParameterName, invalidDestinationFolderPath }
            };

            var invalidApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = invalidSourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(invalidApplicationSettings), _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{invalidSourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_Without_Parameters_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>();

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = sourceFolderPath
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{sourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Valid_DestinationFolder_Parameter_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = string.Empty
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{string.Empty}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_SourceFolder_And_Valid_DestinationFolder_Parameter_And_Valid_SourceFolder_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var invalidSourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.SourceFolderParameterName, invalidSourceFolderPath },
                { MoveAllFilesCommand.DestinationFolderParameterName, destinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{invalidSourceFolderPath}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_DestinationFolder_Parameter_And_Invalid_SourceFolder_Settings_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var invalidDestinationFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.DestinationFolderParameterName, invalidDestinationFolderPath }
            };

            var validApplicationSettings = new ApplicationSettings()
            {
                DefaultSourceFolderPath = string.Empty
            };
            var sut = new MoveAllFilesCommand(GetOptions(validApplicationSettings), _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{string.Empty}' does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Invalid_DestinationFolder_Parameter_And_Valid_SourceFolder_Parameters_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var invalidDestinationFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            Environment.CurrentDirectory = testFolderFullPath;
            var parameters = new Dictionary<string, string>
            {
                { MoveAllFilesCommand.DestinationFolderParameterName, invalidDestinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(parameters));

            // Assert
            Assert.Equal($"ERROR: Directory '{invalidDestinationFolderPath}' does not exists.", exception.Message);
        }

        ~MoveAllFilesCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
