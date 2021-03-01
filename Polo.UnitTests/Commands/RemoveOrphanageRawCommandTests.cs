using FluentAssertions;
using Microsoft.Extensions.Options;
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
using System.Linq;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class RemoveOrphanageRawCommandTests : CommandTestBase
    {
        private static readonly string _albumName = "Album1";
        private static readonly string _rawFolderName = "RAW";
        private static readonly ICollection<string> _rawFileExtensions = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpg", "jpeg" };
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            JpegFileExtensions = _jpegFileExtensions,
            RawFileExtensions = _rawFileExtensions,
            RawFolderName = _rawFolderName
        };
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new RemoveOrphanageRawCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

        private readonly Folder _folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "JPEG")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "ORF"),
                                new FotoFile("UTP-2", "ORF"),
                                new FotoFile("UTP-3", "ORF"),
                                new FotoFile("UTP-4", "ORF"),
                                new FotoFile("UTP-5", "ORF"),
                                new FotoFile("UTP-6", "ORF"),
                            }
                        }
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
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "JPEG")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "ORF"),
                                new FotoFile("UTP-2", "ORF")
                            }
                        }
                    }
                }
            }
        };

        public RemoveOrphanageRawCommandTests()
        {
            AddRawFilesToStructure();
        }

        private void AddRawFilesToStructure()
        {
            var rawFiles = _rawFileExtensions.Select(extension => new FotoFile($"image-{extension}", extension)).ToList();

            _folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x => x.SubFolders.Where(folder => folder.Name == _rawFolderName).ToList()
                    .ForEach(folder => folder.Files.AddRange(rawFiles)));
        }

        [Fact]
        public void Action_Should_Create_Raw_Folder_And_Move_Raw_Files_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            // Act
            _sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }






        [Fact]
        public void Action_Should_Create_Raw_Folder_And_Move_Raw_Files_If_Setting_Have_Duplicate_JpegFileExtension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var jpegFileExtensionsWithDuplicates = new List<string>() { "jpeg", "jpg", "jpeg", "jpg" };
            var applicationSettings = new ApplicationSettings()
            {
                JpegFileExtensions = jpegFileExtensionsWithDuplicates,
                RawFileExtensions = _rawFileExtensions,
                RawFolderName = _rawFolderName
            };
            var sut = new RemoveOrphanageRawCommand(GetOptions(applicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void AAction_Should_Create_Raw_Folder_And_Move_Raw_Files_If_Setting_Have_Duplicate_RawFileExtension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var rawFileExtensionsWithDuplicates = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f",
                                                                       "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
            var applicationSettings = new ApplicationSettings()
            {
                JpegFileExtensions = _jpegFileExtensions,
                RawFileExtensions = rawFileExtensionsWithDuplicates,
                RawFolderName = _rawFolderName
            };
            var sut = new RemoveOrphanageRawCommand(GetOptions(applicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        ~RemoveOrphanageRawCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
