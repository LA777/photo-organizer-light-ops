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
    public class ResizeCommandTests : CommandTestBase
    {
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpeg", "jpg" };
        private static readonly int _imageResizeLongSideLimit = 100;
        private static readonly float _megaPixelsLimit = 16.0f;
        private static readonly string _resizedImageSubfolderName = "small";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = _jpegFileExtensions,
            ImageResizeLongSideLimit = _imageResizeLongSideLimit,
            ImageResizeMegaPixelsLimit = _megaPixelsLimit,
            ResizedImageSubfolderName = _resizedImageSubfolderName
        };
        private static readonly string _albumName = "Album1";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new ResizeCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

        private readonly Folder _folderStructureInitial = new Folder()
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
                        new FotoFile("UTP-11", "jpeg", 5000, 4000),
                        new FotoFile("UTP-12", "jpeg", 5542, 3334),
                        new FotoFile("UTP-1", "jpg", 90, 30),
                        new FotoFile("UTP-2", "jpeg", 30, 90),
                        new FotoFile("UTP-3", "jpeg", 120, 90),
                        new FotoFile("UTP-4", "jpeg", 90, 120),
                        new FotoFile("UTP-5", "jpeg", 120, 120),
                        new FotoFile("UTP-6", "jpeg", 100, 90),
                        new FotoFile("UTP-7", "jpeg", 90, 100),
                        new FotoFile("UTP-8", "jpeg", 100, 100)
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
                        new FotoFile("video-1", "mp4"),
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-1", "jpg", 90, 30),
                        new FotoFile("UTP-2", "jpeg", 30, 90),
                        new FotoFile("UTP-3", "jpeg", 120, 90),
                        new FotoFile("UTP-4", "jpeg", 90, 120),
                        new FotoFile("UTP-5", "jpeg", 120, 120),
                        new FotoFile("UTP-6", "jpeg", 100, 90),
                        new FotoFile("UTP-7", "jpeg", 90, 100),
                        new FotoFile("UTP-8", "jpeg", 100, 100)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _resizedImageSubfolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "jpg", 90, 30),
                                new FotoFile("UTP-2", "jpeg", 30, 90),
                                new FotoFile("UTP-3", "jpeg", 100, 75),
                                new FotoFile("UTP-4", "jpeg", 75, 100),
                                new FotoFile("UTP-5", "jpeg", 100, 100),
                                new FotoFile("UTP-6", "jpeg", 100, 90),
                                new FotoFile("UTP-7", "jpeg", 90, 100),
                                new FotoFile("UTP-8", "jpeg", 100, 100)
                            }
                        }
                    }
                }
            }
        };

        [Fact]
        public void Action_Should_Resize_Jpeg_Files_And_Copy_To_Output_Folder_Test()
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
        public void Action_Should_Resize_Jpeg_Files_And_Copy_To_Output_Folder_With_Valid_LongSideLimitParameter_And_Valid_ImageResizeLongSideLimit_Setting_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);
            const string validLongSideLimitParameter = "100";

            var parameters = new Dictionary<string, string>
            {
                { LongSideLimitParameter.Name, validLongSideLimitParameter }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Resize_Jpeg_Files_And_Copy_To_Output_Folder_With_Valid_MegaPixelsLimitParameter_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);
            const string validLongSideLimitParameter = "10000";

            var parameters = new Dictionary<string, string>
            {
                { LongSideLimitParameter.Name, validLongSideLimitParameter }
            };

            // Act
            _sut.Action(parameters);

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Resize_Jpeg_Files_And_Copy_To_Output_Folder_If_Setting_Have_Duplicate_Extension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var jpegFileExtensionsWithDuplicates = new List<string>() { "jpeg", "jpg", "jpeg", "jpg" };
            var applicationSettings = new ApplicationSettings()
            {
                FileForProcessExtensions = jpegFileExtensionsWithDuplicates,
                ImageResizeLongSideLimit = _imageResizeLongSideLimit,
                ResizedImageSubfolderName = _resizedImageSubfolderName
            };

            var sut = new ResizeCommand(GetOptions(applicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        ~ResizeCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
