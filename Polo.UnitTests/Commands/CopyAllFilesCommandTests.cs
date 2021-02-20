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
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class CopyAllFilesCommandTests : CommandTestBase
    {
        private const string DefaultSourceDriveName = "e:\\\\";
        private readonly string SourceFolderArgumentName = "source";
        private readonly string DestinationFolderArgumentName = "destination";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings(defaultSourceDriveName: DefaultSourceDriveName);
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly string _sourceFolderName = "Source Fotos";
        private static readonly string _destinationFolderName = "Destination Fotos";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new CopyAllFilesCommand(_mockApplicationOptions, _loggerMock.Object);

        private readonly Folder folderStructureInitial = new Folder()
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

        private readonly Folder folderStructureExpected = new Folder()
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
        public void Action_Should_Copy_Files_With_Two_Arguments_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath },
                { DestinationFolderArgumentName, destinationFolderPath }
            };

            // Act
            _sut.Action(arguments);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Copy_Files_With_Two_Arguments_If_Path_Contains_Whitespace_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath },
                { DestinationFolderArgumentName, destinationFolderPath }
            };

            // Act
            _sut.Action(arguments);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Two_Arguments_And_Fail_Source_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = testFolderFullPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath },
                { DestinationFolderArgumentName, destinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(arguments));

            // Assert
            Assert.Equal($"Directory {sourceFolderPath} does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_Two_Arguments_And_Fail_Destination_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            Environment.CurrentDirectory = testFolderFullPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath },
                { DestinationFolderArgumentName, destinationFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(arguments));

            // Assert
            Assert.Equal($"Directory {destinationFolderPath} does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Copy_Files_With_One_Arguments_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath }
            };

            // Act
            _sut.Action(arguments);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_With_One_Arguments_And_Fail_Source_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var arguments = new Dictionary<string, string>
            {
                { SourceFolderArgumentName, sourceFolderPath }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Action(arguments));

            // Assert
            Assert.Equal($"Directory {sourceFolderPath} does not exists.", exception.Message);
        }

        [Fact]
        public void Action_Should_Copy_Files_Without_Argument_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, _sourceFolderName);
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var arguments = new Dictionary<string, string>();

            ApplicationSettings _validApplicationSettings = new ApplicationSettings(defaultSourceDriveName: sourceFolderPath);
            IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
            var sut = new CopyAllFilesCommand(_mockApplicationOptions, _loggerMock.Object);

            // Act
            sut.Action(arguments);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Throw_Exception_Without_Argument_And_Fail_Source_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            var sourceFolderPath = Path.Combine(testFolderFullPath, "no such folder");
            var destinationFolderPath = Path.Combine(testFolderFullPath, _destinationFolderName);
            Environment.CurrentDirectory = destinationFolderPath;
            var arguments = new Dictionary<string, string>();

            ApplicationSettings _validApplicationSettings = new ApplicationSettings(defaultSourceDriveName: sourceFolderPath);
            IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
            var sut = new CopyAllFilesCommand(_mockApplicationOptions, _loggerMock.Object);

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => sut.Action(arguments));

            // Assert
            Assert.Equal($"Directory {sourceFolderPath} does not exists.", exception.Message);
        }

        ~CopyAllFilesCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
