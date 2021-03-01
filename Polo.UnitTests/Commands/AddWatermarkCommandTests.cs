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
    public class AddWatermarkCommandTests : CommandTestBase
    {
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpeg", "jpg" };
        private static readonly string _watermarkOutputFolderName = "watermark-output";
        private static readonly string _watermarkFolderName = "sign";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            JpegFileExtensions = _jpegFileExtensions,
            WatermarkPath = "",
            WatermarkOutputFolderName = _watermarkOutputFolderName,
            WatermarkPosition = "center-center",
            WatermarkTransparencyPercent = 90
        };

        private static readonly string _albumName = "Album1";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new AddWatermarkCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

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
                        new FotoFile("UTP-1", "jpg", 120, 90),
                        new FotoFile("UTP-2", "jpeg", 200, 100)
                    }
                },
                new Folder()
                {
                    Name = _watermarkFolderName,
                    Files = new List<FotoFile>()
                    {
                        FileHelper.Watermark
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
                        new FotoFile("UTP-1", "jpg", 120, 90),
                        new FotoFile("UTP-2", "jpeg", 200, 100)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _watermarkOutputFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "jpg", 120, 90),
                                new FotoFile("UTP-2", "jpeg", 200, 100)
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = _watermarkFolderName,
                    Files = new List<FotoFile>()
                    {
                        FileHelper.Watermark
                    }
                }
            }
        };

        [Fact]
        public void Action_Should_Add_Watermark_And_Copy_To_Output_Folder_Test()
        {
            // TODO LA - Complete
            // TODO LA - Check whether watermark been added

            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            // TODO LA - Refactor
            _validApplicationSettings.WatermarkPath = Path.Combine(testFolderFullPath, _watermarkFolderName, FileHelper.Watermark.GetNameWithExtension());
            var sut = new AddWatermarkCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);


            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        // TODO LA - Complete



        ~AddWatermarkCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
